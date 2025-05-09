<template>
  <li class="card_scroll_container">
    <p v-if="isHighlighted" class="most_liked_hint">Publikumsliebling</p>
    <div :class="{ topic_card_header: true, most_liked_highlight: isHighlighted }">
      <button class="action_button" @click="$emit('toggle-details')">
        <img :src="topic.expanded ? 'collapse.svg' : '/expand.svg'" alt="Expand">
      </button>
      <h4 :class="{ topic_card_header_toggled: topic.expanded }">{{ topic.title }}</h4>
      <slot name="action-button"></slot>
    </div>
    <div v-if="topic.expanded" class="topic-card-details">
      <div style="display: flex; flex-direction: row;">
        <p class="topic-card-presentation-time-in-minutes">{{ topic.presentationTimeInMinutes }}'</p>
        <p class="description">{{ topic.description }}</p>
        <slot name="actions"></slot>
      </div>
      <slot name="presenter"></slot>
    </div>
    <div v-if="topic.comments && topic.comments.length > 0">
      <p style="margin-left: 3rem; margin-top: .1rem">Kommentare:</p>
      <ul style="margin-bottom: 1rem; margin-left: .5rem">
        <div v-for="comment in topic.comments" :key="comment.createdAt"
             style="max-width: 90%; margin-left: 0.5rem; font-size: small; margin-top: 0.25rem">
          <div style="display: flex; flex-direction: row">
            <p style="font-weight: bold">{{ comment.creatorUserName }}</p>
            <p style="margin-left: .25rem">({{ formatDateTime(comment.createdAt) }})</p>
          </div>
          <p style="margin-left: 1rem; margin-top: 0.15rem">{{ comment.content }}</p>
        </div>
      </ul>
      <div style="display: flex; flex-direction: row">
        <input v-model="content"
               style="border-color: var(--main-color-primary);color: var(--main-color-primary);margin-left: 3rem; border-style: solid; border-width: 0.01rem; border-radius: 0.2rem;"
               placeholder="Kommentar schreiben ..."/>
        <button class="action_button" style="color: var(--color-primary)"
                @click="sendComment(topic.id)">Senden
        </button>
      </div>
    </div>

  </li>
</template>

<script lang="ts">
import {defineComponent, type PropType} from 'vue';
import type {MyTopic, ForeignTopic} from '@/composables/TopicInterfaces';
import axios from "axios";

export default defineComponent({
  data() {
    return {
      content: '',
    };
  },
  props: {
    topic: {
      type: Object as PropType<MyTopic | ForeignTopic>,
      required: true
    },
    isHighlighted: {
      type: Boolean,
      default: false
    }
  },
  emits: ['toggle-details'],
  methods: {
    sendComment(topicId: string){
      axios.post('api/topic/CommentOnTopic/', { TopicId: topicId, Content: this.content })
    },
    axios,
    formatDateTime(dateTimeString: string): string {
      const date = new Date(dateTimeString);
      return new Intl.DateTimeFormat('de-DE', {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
      }).format(date);
    }
  }
});
</script>

<style scoped src="src/assets/topics.css"></style>
<style scoped src="src/assets/instructions.css"></style>
