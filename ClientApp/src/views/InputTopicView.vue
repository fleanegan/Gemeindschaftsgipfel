<template>
  <h1>{{ isEditing ? 'Thema bearbeiten' : 'Neues Thema hinzufügen' }}</h1>
  <form @submit.prevent="submitData">
    <div class="form-group">
      <label for="title">Worum geht es?</label>
      <input id="title" v-model="title" class="form-input" type="text"/>
      <p v-if="isTitleEmpty" class="title-error">Der Titel muss zwischen 1 und 150 Zeichen lang sein</p>
    </div>
    <div class="form-group">
      <label for="description">Details und benötigtes Material</label>
      <input id="description" v-model="description" class="form-input"/>
    </div>
    <div class="form-group">
      <label for="presentationTimeInMinutes">Wie lange dauert dein Beitrag in etwa?</label>
      <input id="presentationTimeInMinutes" type='number' v-model.number="presentationTimeInMinutes" class="form-input" placeholder="in Minuten"/>
    </div>
    <div class="button-container">
      <button class="abort-button" @click="abort">Verwerfen</button>
      <button class="submit-button" type="submit">Abschicken</button>
    </div>
  </form>
</template>

<script lang="ts">
import {defineComponent} from 'vue';
import axios from "axios";

export default defineComponent({
  data() {
    return {
      title: '',
      description: '',
      presentationTimeInMinutes: null,
    };
  },
  methods: {
    isTopicIdSet(): boolean {
      return this.$props['topicId'] !== undefined && this.$props['topicId'] !== null;
    },
    async submitData() {
      if (this.isTitleEmpty) {
        return;
      }
      try {
        if (this.isEditing) {
          await axios.put('/api/topic/update', {
            "Title": this.title,
	    "PresentationTimeInMinutes": this.presentationTimeInMinutes,
            "Description": this.description,
            "Id": this.$props["topicId"],
          });
        } else {
          await axios.post('/api/topic/addnew', {
            "Title": this.title,
	    "PresentationTimeInMinutes": this.presentationTimeInMinutes,
            "Description": this.description,
          });
        }

        this.$router.push('/topic');
      } catch (e) {
        console.log("error while sending topic: ", e)
      }
    },
    async abort() {
      this.$router.push('/topic');
    }
  },
  computed: {
    isTitleEmpty() {
      return this.title.length === 0
    },
    isEditing() {
      return this.isTopicIdSet();
    }
  },
  async beforeCreate() {
    // why is this.isEditing not working?
    if (this.$props['topicId'] !== undefined && this.$props['topicId'] !== null) {
      try {
        var existingTopic = await axios.get('/api/topic/getone/' + this.$props["topicId"]);
        this.title = existingTopic.data["title"];
        this.description = existingTopic.data["description"];
        this.presentationTimeInMinutes = existingTopic.data["presentationTimeInMinutes"];
      } catch (e) {
        console.log("edit: could not get existing topic")
      }
    }
  },
  props: ['topicId'],
});
</script>

<style scoped>

.switch input {
  position: absolute;
  opacity: 0;
}

.form-group {
  display: flex;
  flex-direction: column;
  padding: 0 1rem 1rem 1rem;
}

.title-error {
  color: red;
}

.button-container {
  display: flex;
  flex-direction: row;
}

.submit-button {
  margin-left: auto;
  margin-right: 1rem;
}
</style>
