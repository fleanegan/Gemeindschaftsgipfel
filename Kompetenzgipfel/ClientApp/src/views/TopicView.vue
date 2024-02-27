<template>
  <div class="about">
    <h1>Vortragsthemen</h1>
    <ul class="list">
      <li v-for="(item, index) in apiResponse" :key="index" class="topic-card">
        <div class="topic-card-header">
          <button class="toggle-button" @click="toggleDetails(index)"><img
              :src="item.expanded ? 'collapse.svg' : '/expand.svg'" alt="Expand">
          </button>
          <h3 :class="{topic_card_header_toggled: item.expanded}">{{ item.title }}</h3>
          <button class="toggle-button" @click="toggleVote(index)"><img
              :src="item.voted ? '/full_heart.svg' : '/empty_heart.svg'" alt="Vote">
          </button>
        </div>
        <div v-if="item.expanded" class="topic-card-details">
          <p class="description">{{ item.description }}</p>
          <p class="presenter">Vortragsmensch: {{ item.presenterUserName }}</p>
        </div>
      </li>
      <div class="topic-card-details">Ende der Liste. Danke fürs Abstimmen!<br></div>
    </ul>
  </div>
</template>


<script lang="ts">
import {defineComponent} from 'vue';
import axios from "axios";


interface ApiResponseItem {
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
      apiResponse: [] as ApiResponseItem[],
    };
  },
  methods: {
    async fetchData() {
      this.apiResponse = (await axios.get('/api/topic/allExceptLoggedIn', {})).data;
    },
    toggleDetails(index: number): void {
      this.apiResponse[index].expanded = !this.apiResponse[index].expanded;
    },
    toggleVote(index: number): void {
      this.apiResponse[index].voted = !this.apiResponse[index].voted;
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

.topic-card-header h3 {
  padding-left: 0;
  margin-right: auto;
}

.topic-card-details {
  padding-left: 3rem;
  padding-top: 0.5rem;
  padding-right: 1rem;
  padding-bottom: 0.5rem;
  background-color: var(--color-border-light);
}

.description {
  padding-right: 4rem;
  font-size: 0.85rem;
}

.presenter {
  padding-top: 0.25rem;
  font-size: 1rem;
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
</style>