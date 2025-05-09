<template>
  <div class="topic">
    <div :class="{'floating_scroll_to_top_hidden': true, 'floating_scroll_to_top_shown': isSticky}">
      <button class="action_button" style="margin-right: 1rem;" @click="scrollToTop">
        <img :src="'/expand.svg'" alt="Expand">
      </button>
    </div>
    <h1>Zeig uns was 'ne Harke ist!</h1>
    <div class="instruction_container">
      <div class="instruction_card">
        <div class="instruction_card_content">
          <p class="instruction_card_content_header_title" style="padding-left: 2.25rem">Inhalt ausdenken</p>
          <p>Reite dein Steckenpferd und zeig' uns, was dich begeistert! Ob
            Trick
            17, dein Promotionsthema oder Haekeltipps, wir sind gespannt.</p>
        </div>
        <div class="instruction_card_enumerator">1.</div>
      </div>
      <div class="instruction_card">
        <div class="instruction_card_content">
          <p class="instruction_card_content_header_title" style="padding-left: 2.25rem">Entscheidung treffen</p>
          <p>Geht es dir wie uns, du kannst dich kaum entscheiden, welches deiner vielen Herzensthemen du praesentieren
            sollst? Trag alle Themen ein, lass die Gemeinschaft waehlen und hilf selbst mit deiner Stimme!</p>
        </div>
        <div class="instruction_card_enumerator">2.</div>
      </div>
      <div class="instruction_card">
        <div class="instruction_card_content">
          <p class="instruction_card_content_header_title" style="padding-left: 2.25rem">Gemeinsam staunen</p>
          <p>Das Ziel ist es, zusammen unsere Vielfalt zu geniessen. Lass Leistungsdruck und Lampenfieber zuhause, denn
            es
            erwartet dich ein wohlwollendes Publikum :)</p>
        </div>
        <div class="instruction_card_enumerator">3.</div>
      </div>
    </div>
    <h2>Meine Vorschläge</h2>
    <ul class="list">
      <TopicCard
          v-for="(item, index) in myTopics"
          :key="item.id"
          :topic="item"
          :isHighlighted="item === mostLikedTopic && mostLikedTopic?.votes > 0"
          @toggle-details="toggleDetails(myTopics, index)"
          @comment-sent="handleCommentSent"
      >
        <template #actions>
          <div class="topic_card_details_owner_actions">
            <button class="action_button" style="margin-bottom: 0.25rem;" @click="removeTopic(item.id)">
              <img alt="Delete" src="/empty_delete.svg">
            </button>
            <button class="action_button" @click="editTopic(item.id)">
              <img alt="Edit" src="/empty_edit_no_border.svg">
            </button>
          </div>
        </template>
      </TopicCard>
      <li>
        <hr>
        <div id="owner_action" class="my-topics-add-button-container">
          <button class="submit-button" @click="addNewTopic">Neue Idee?</button>
        </div>
      </li>
    </ul>
    <h2>Ideen der Anderen</h2>
    <ul class="list">
      <TopicCard
          v-for="(item, index) in foreignTopics"
          :key="item.id"
          :topic="item"
          :isHighlighted="false"
          @toggle-details="toggleDetails(foreignTopics, index)"
          @comment-sent="handleCommentSent"
      >
        <template #action-button>
          <button class="action_button" @click="toggleVote(index)">
            <img :src="item.didIVoteForThis ? '/full_heart.svg' : '/empty_heart.svg'" alt="Vote">
          </button>
        </template>
        <template #presenter>
          <p class="presenter">{{ item.presenterUserName }}</p>
        </template>
      </TopicCard>
      <li class="topic-card-details">Ende der Liste. Danke fürs Abstimmen!
        <br style="margin-bottom: 25rem">
      </li>
    </ul>
  </div>
</template>


<script lang="ts">
import {defineComponent} from 'vue';
import axios from "axios";
import type {ForeignTopic, MyTopic, Comment} from "@/composables/TopicInterfaces";
import TopicCard from "@/composables/TopicCard.vue";


export default defineComponent({
  components: {TopicCard},
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
async toggleDetails(topic: MyTopic[] | ForeignTopic[], index: number): Promise<void> {
  if (topic[index].isLoading) return;
  try {
    topic[index].expanded = !topic[index].expanded;
    if (topic[index].expanded) {
      topic[index].isLoading = true;
      const response = await axios.get('/api/topic/comments?TopicId=' + topic[index].id);
      topic[index].comments = response.data;

      topic[index].comments.sort((a: Comment, b: Comment) => {
        return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
      });
    } else {
      topic[index].comments = [];
    }
  } catch (error) {
    console.error('Error toggling details:', error);
    // Revert expanded state on error
    topic[index].expanded = !topic[index].expanded;
  } finally {
    // Clear loading state
    topic[index].isLoading = false;
  }
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
        name: 'Thema bearbeiten',
        params: {
          'topicId': topicId,
        }
      });
    },
    formatDateTime(dateTimeString: string): string {
      const date = new Date(dateTimeString);
      return new Intl.DateTimeFormat('de-DE', {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
      }).format(date);
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
    },
    async handleCommentSent(payload: { topicId: string, comment: Comment, content: string }): Promise<void> {
      const updateTopicComments = (topics: (MyTopic | ForeignTopic)[]) => {
        const topicIndex = topics.findIndex(t => t.id === payload.topicId);
        if (topicIndex !== -1 && topics[topicIndex].expanded) {
          if (payload.comment) {
            topics[topicIndex].comments.unshift(payload.comment);
          } else {
            this.refreshTopicComments(topics[topicIndex]);
          }
        }
      };

      updateTopicComments(this.myTopics);
      updateTopicComments(this.foreignTopics);
    },
    async refreshTopicComments(topic: MyTopic | ForeignTopic): Promise<void> {
      if (topic.expanded) {
        topic.comments = (await axios.get('/api/topic/comments?TopicId=' + topic.id)).data;
        topic.comments.sort((a: Comment, b: Comment) => {
          return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
        });
      }
    },
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

