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
      this.apiResponse = [
        {
          "description": "Es geht um die Erfindung der Mausbaumbeschlagslackierung",
          "id": "da1124c6-b7f2-4b76-af2b-ec4dfad362aa",
          "presenterUserName": "Sabsabsab",
          "title": "Mausbaumbeschlagslackierung",
          expanded: false,
          voted: false,
        },
        {
          "description": "Wie geht es wohl weiter im schwedischen Koenigshaus. es gibt einiges zu bestaunen bei den skandinavischen royals",
          "id": "3072bf9c-ddd3-4c9b-b0a8-34800d047c94",
          "presenterUserName": "Sabsabsab",
          "title": "Drana im schwedischen Koenigshaus",
          expanded: false,
          voted: false,
        },
        {
          "description": "Die neue Saison beinhaltet viele spannende Spaesse fuer die ganze Familie mit Thomas Raab",
          "id": "862335fb-6c75-4bbe-bebe-57ef04bc7667",
          "presenterUserName": "Sabsabsab",
          "title": "Top die Wette gilt",
          expanded: false,
          voted: false,
        },
        {
          "description": "Hier fahren alle ein bisschen langsamer aber das sollte ja hier niemanden stoeren, wo wir tempo 130 auf der autobahn wollen. Denn am Ende des Tages lautet die Frage immernoch, wie kriegen wir den Speck auf den Tisch. Reich gedeckt m'chte er sein aber im Lotto gewonnen will ich daf[r auch nicht haben m[ssen.",
          "id": "d79f49db-eaa3-4672-a642-fae2abf2ae9e",
          "presenterUserName": "Sabsabsab",
          "title": "Formel 3",
          expanded: false,
          voted: false,
        }
      ];
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