<template>
  <div class="topic">
    <div :class="{'floating_scroll_to_top_hidden': true, 'floating_scroll_to_top_shown': isSticky}">
      <button class="action_button" style="margin-right: 1rem;" @click="scrollToTop">
        <img :src="'/expand.svg'" alt="Expand">
      </button>
    </div>
    <h1>Vortragsthemen</h1>
    <div class="instruction_container">
      <div class="instruction_card">
        <div class="instruction_card_content">
          <p class="instruction_card_content_header_title" style="padding-left: 2.25rem">Inhalt</p>
          <p class="instruction_card_content_header_title" style="padding-left: 1.5rem; padding-bottom: 0.5rem">
            ausdenken</p>
          <p>Reite dein Steckenpferd und erzaehle uns, was dich begeistert! Ob
            Trick
            17, dein Promotionsthema oder Haekeltipps, wir sind gespannt.</p>
        </div>
        <div class="instruction_card_enumerator">1.</div>
      </div>
      <div class="instruction_card">
        <div class="instruction_card_content">
          <p class="instruction_card_content_header_title" style="padding-left: 2.25rem">Entscheidung</p>
          <p class="instruction_card_content_header_title" style="padding-left: 1.5rem; padding-bottom: 0.5rem">
            treffen</p>
          <p>Geht es dir wie uns, du kannst dich kaum entscheiden, welches deiner vielen Herzensthemen du praesentieren
            sollst? Trag alle Themen ein, lass die Gemeindschaft waehlen und hilf selbst mit deiner Stimme!</p>
        </div>
        <div class="instruction_card_enumerator">2.</div>
      </div>
      <div class="instruction_card">
        <div class="instruction_card_content">
          <p class="instruction_card_content_header_title" style="padding-left: 2.25rem">Gemeinsam</p>
          <p class="instruction_card_content_header_title" style="padding-left: 1.5rem; padding-bottom: 0.5rem">
            staunen</p>
          <p>Das Ziel ist es, zusammen unsere Vielfalt zu geniessen. Lass Leistungsdruck und Lampenfieber zuhause, denn
            es
            erwartet dich ein wohlwollendes Publikum :)</p>
        </div>
        <div class="instruction_card_enumerator">3.</div>
      </div>
    </div>
    <h2>Meine Vorschläge</h2>
    <ul class="list">
      <li v-for="(item, index) in myTopics" :key="item.votes" class="topic-card">
        <p v-if="item===mostLikedTopic && mostLikedTopic.votes > 0" class="most_liked_hint">Publikumsliebling</p>
        <div :class="{topic_card_header:true, most_liked_highlight:item===mostLikedTopic && mostLikedTopic.votes > 0}">
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
        <div id="owner_action" class="my-topics-add-button-container">
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
      <li class="topic-card-details">Ende der Liste. Danke fürs Abstimmen!
        <br style="margin-bottom: 25rem">
      </li>
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
      isSticky: false,
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
    },
    handleScroll: function () {
      this.isSticky = window.scrollY > 750;
    },
    scrollToTop() {
      scrollTo(0, 0);
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
    window.addEventListener('scroll', this.handleScroll);
  },
  beforeUnmount() {
    window.removeEventListener('scroll', this.handleScroll);
  },
});
</script>

<style scoped>
.floating_scroll_to_top_hidden {
  margin-left: auto;
  height: 3.3rem;
  width: 75%;
  position: sticky;
  top: 0;
  left: 0;
  z-index: 9999;
  display: none;
}

.floating_scroll_to_top_shown {
  display: flex;
  flex-direction: row;
  align-content: center;
  justify-content: right;
  background-color: transparent;
}

@media (max-width: 900px) {
  .floating_scroll_to_top_shown {
    background-color: var(--color-background);
  }
}

</style>
<style scoped src="src/assets/topics.css"></style>
<style scoped src="src/assets/instructions.css">
</style>