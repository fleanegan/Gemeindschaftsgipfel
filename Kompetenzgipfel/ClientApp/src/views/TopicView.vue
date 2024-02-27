<template>
  <div class="topic">
    <h1>Vortragsthemen</h1>
    <h2>Meine Vorschläge</h2>
    <ul class="list">
      <li v-for="(item, index) in myTopics" :key="index" class="topic-card">
        <div class="topic-card-header">
          <button class="toggle-button" @click="toggleDetails(myTopics, index)"><img
              :src="item.expanded ? 'collapse.svg' : '/expand.svg'" alt="Expand">
          </button>
          <h4 :class="{topic_card_header_toggled: item.expanded}">{{ item.title }}</h4>
        </div>
        <div v-if="item.expanded" class="topic-card-details">
          <p class="description">{{ item.description }}</p>
        </div>
      </li>
      <li>
        <hr>
        <div class="my-topics-add-button-container">
          <button class="submit-button" @click="addNewTopic">Neue Idee?</button>
        </div>
      </li>
    </ul>
    <h2>Das haben sich die Anderen ausgedacht</h2>
    <ul class="list">
      <li v-for="(item, index) in foreignTopics" :key="index" class="topic-card">
        <div class="topic-card-header">
          <button class="toggle-button" @click="toggleDetails(foreignTopics, index)"><img
              :src="item.expanded ? 'collapse.svg' : '/expand.svg'" alt="Expand">
          </button>
          <h4 :class="{topic_card_header_toggled: item.expanded}">{{ item.title }}</h4>
          <button class="toggle-button" @click="toggleVote(index)"><img
              :src="item.voted ? '/full_heart.svg' : '/empty_heart.svg'" alt="Vote">
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
  expanded: boolean;
}

interface ForeignTopic {
  id: string;
  title: string;
  description: string;
  presenterUserName: string
  expanded: boolean;
  voted: boolean;
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
    toggleVote(index: number): void {
      this.foreignTopics[index].voted = !this.foreignTopics[index].voted;
    },
    editTopic(index: number): void {

    },
    addNewTopic() {
      this.$router.push("/topic/add");
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
  padding: 1rem;
}

.topic-card {
  display: flex;
  flex-direction: column;
}

.topic-card-header {
  display: flex;
  flex-direction: row;
  align-items: center;
  align-content: center;
  border: 0.1rem;
  border-style: solid;
  border-color: var(--color-border-light);
  border-radius: 0.1rem;
  border-bottom: none;
}

.topic-card-header button img {
  width: 1.35rem;
  height: 1.35rem;
  margin: 0.5rem;
}

.topic-card-header h4 {
  padding-left: 0;
  margin-right: auto;
}

.topic-card-details {
  padding-left: 3rem;
  padding-top: 0.5rem;
  padding-right: 0;
  padding-bottom: 0.5rem;
  background-color: var(--main--color-nuance-light);
}

.description {
  padding: 0.5rem;
  margin-right: 3rem;
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

.toggle-button {
  cursor: pointer;
  background: none;
  border: 0.01rem;
  border-style: solid;
  border-color: var(--color-background);
  color: var(--main-color-text-light);
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