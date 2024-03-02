<template>
  <div class="topic">
    <h1>Vortragsthemen</h1>
    <h2>Meine Vorschläge</h2>
    <ul class="list">
      <li v-for="(item, index) in myTopics" :key="item.votes" class="topic-card">
        <p v-if="item==mostLikedTopic" class="most_liked_hint">Publikumsliebling</p>
        <div :class="{topic_card_header:true, most_liked_highlight:item===mostLikedTopic}">
          <button class="action_button" @click="toggleDetails(myTopics, index)">
            <img :src="item.expanded ? 'collapse.svg' : '/expand.svg'" alt="Expand">
          </button>
          <h4 :class="{topic_card_header_toggled: item.expanded}">{{ item.title }}</h4>
        </div>
        <div v-if="item.expanded" class="topic-card-details">
          <div class="topic_card_details_owner">
            <p class="description">{{ item.description }}</p>
            <div class="topic_card_details_owner_actions">
              <button class="action_button" style="margin-bottom: 0.25rem;" @click="removeTopic(item.id)">
                <img alt="Expand" src='/empty_delete.svg'>
              </button>
              <button class="action_button" @click="editTopic(item.id)">
                <img alt="Expand" src='/empty_edit_no_border.svg'>
              </button>
            </div>
          </div>
        </div>
      </li>
      <li>
        <hr>
        <div class="my-topics-add-button-container">
          <button class="submit-button" @click="addNewTopic">Neue Idee?</button>
        </div>
      </li>
    </ul>
    <h2>Ideen der Anderen</h2>
    <ul class="list">
      <li v-for="(item, index) in foreignTopics" :key="index" class="topic-card">
        <div class="topic_card_header">
          <button class="action_button" @click="toggleDetails(foreignTopics, index)"><img
              :src="item.expanded ? 'collapse.svg' : '/expand.svg'" alt="Expand">
          </button>
          <h4 :class="{topic_card_header_toggled: item.expanded}">{{ item.title }}</h4>
          <button class="action_button" @click="toggleVote(index)"><img
              :src="item.didIVoteForThis ? '/full_heart.svg' : '/empty_heart.svg'" alt="Vote">
          </button>
        </div>
        <div v-if="item.expanded" class="topic-card-details">
          <p class="description">{{ item.description }}</p>
          <p class="presenter">{{ item.presenterUserName }}</p>
        </div>
      </li>
      <hr>
      <li class="topic-card-details">Ende der Liste. Danke fürs Abstimmen!<br></li>
    </ul>
  </div>
</template>


<script lang="ts">
import {defineComponent} from 'vue';
import axios from "axios";

interface MyTopic {
  id: string;
  title: string;
  description: string;
  votes: number;
  expanded: boolean;
}

interface ForeignTopic {
  id: string;
  title: string;
  description: string;
  presenterUserName: string;
  didIVoteForThis: boolean;
  expanded: boolean;
}

export default defineComponent({
  data() {
    return {
      foreignTopics: [] as ForeignTopic[],
      myTopics: [] as MyTopic[],
    };
  },
  methods: {
    async fetchData() {
      this.myTopics = (await axios.get('/api/topic/allOfLoggedIn', {})).data
      this.foreignTopics = (await axios.get('/api/topic/allExceptLoggedIn', {})).data;
    },
    toggleDetails(topic: MyTopic[] | ForeignTopic[], index: number): void {
      topic[index].expanded = !topic[index].expanded;
    },
    async toggleVote(index: number): Promise<void> {
      if (!this.foreignTopics[index].didIVoteForThis)
        await axios.post('/api/topic/addVote', {"TopicId": this.foreignTopics[index].id})
      else
        await axios.delete('/api/topic/removeVote/' + this.foreignTopics[index].id)
      this.foreignTopics[index].didIVoteForThis = !this.foreignTopics[index].didIVoteForThis;
    },
    editTopic(topicId: string): void {
      this.$router.push({
        name: 'Vortragsthema bearbeiten',
        params: {
          'topicId': topicId,
        }
      });
    },
    async removeTopic(topicId: string) {
      await axios.delete('api/topic/delete/' + topicId);
      await this.fetchData();
    },
    addNewTopic() {
      this.$router.push("/topic/add");
    }
  },
  computed: {
    mostLikedTopic() {
      if (!this.myTopics.length)
        return null;
      let result = this.myTopics[0]
      for (const topic of this.myTopics) {
        if (topic.votes > result.votes)
          result = topic;
      }
      return result;
    }
  },
  mounted() {
    this.fetchData()
  },
});
</script>

<style scoped>

.list {
  list-style: none;
  margin-right: 2rem;
  margin-bottom: 0;
  margin-top: 0;
  padding-left: 1rem;
  padding-right: 1rem;
}

.topic-card {
  display: flex;
  flex-direction: column;
}

.most_liked_hint {
  margin-left: 1.5rem;
  margin-top: 0.5rem;
  background-color: var(--color-highlight);
  border-top-left-radius: 0.2rem;
  border-top-right-radius: 0.2rem;
  width: 12rem;
  height: 2rem;
  text-align: center;
  color: var(--color-background);
}


.topic_card_header {
  display: flex;
  min-height: 3rem;
  flex-direction: row;
  align-items: center;
  align-content: center;
  border-left: .25rem solid var(--color-background);
}

/*do not delete*/
.most_liked_highlight {
  border-radius: 1rem;
  border: .25rem solid var(--color-highlight);
}

.topic_card_header button img {
  width: 1.35rem;
  height: 1.35rem;
  margin: 0.5rem;
}

.topic_card_header h4 {
  padding-left: 0;
  margin-right: auto;
}

.topic-card-details {
  padding-left: 3rem;
  padding-top: 0.5rem;
  padding-right: 0.5rem;
  padding-bottom: 0.5rem;
  background-color: var(--main--color-nuance-light);
  border-radius: 0.2rem;
}

.topic_card_details_owner {
  display: flex;
  flex-direction: row;
}

.topic_card_details_owner_actions {
  display: flex;
  flex-direction: column;
  align-content: center;
  align-items: center;
}

.topic_card_details_owner_actions button {
  margin-left: 0.5rem;
}

.description {
  padding: 0.5rem;
  margin-right: 0;
  width: 100%;
  border: 0.1rem;
  border-style: solid;
  border-color: var(--main-color-border-light);
  border-radius: 0.15rem;
  font-size: 0.85rem;
}

.presenter {
  padding-top: 0.25rem;
  margin-right: 1rem;
  font-size: 0.9rem;
  text-align: end;
}

.action_button {
  cursor: pointer;
  background: none;
  border-style: none;
  border-radius: 0.2rem;
  font-weight: bold;
  display: flex;
  place-items: center;
}


.my-topics-add-button-container {
  display: flex;
  flex-direction: row;
  justify-content: flex-end;
}

.submit-button {
  margin-top: 0;
  margin-left: auto;
  border-top-left-radius: 0;
  border-top-right-radius: 0;
  margin-right: 0;
}

ul {
  margin-bottom: 5rem;
}
</style>