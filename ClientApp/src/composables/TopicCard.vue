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
      <div class="comments-container">
        <p class="comments-title">Kommentare:</p>
        <ul class="comments-list">
          <div v-for="comment in topic.comments" :key="comment.createdAt" class="comment-item">
            <div class="flex-row">
              <p class="comment-author">{{ comment.creatorUserName }}</p>
              <p class="comment-timestamp">({{ formatDateTime(comment.createdAt) }})</p>
            </div>
            <p class="comment-content">{{ comment.content }}</p>
          </div>
        </ul>
        <div class="flex-row" style="margin-bottom: 2rem">
          <input v-model="content"
                 class="comment-input"
                 placeholder="Kommentar schreiben ..."/>
          <button class="action_button comment-send-button"
                  @click="sendComment(topic.id)">Senden
          </button>
        </div>
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
  emits: ['toggle-details', 'comment-sent'],
  methods: {
    async sendComment(topicId: string) {
      try {
        if (!this.content) {
          return;
        }
        const response = await axios.post('api/topic/CommentOnTopic/', {TopicId: topicId, Content: this.content});
        this.$emit('comment-sent', {
          topicId: topicId,
          comment: response.data,
          content: this.content
        });
        this.content = '';
      } catch (error) {
        console.error('Error sending comment:', error);
      }
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
<style scoped>
.comments-container {
  margin-top: 0.5rem;
}

.comments-title {
  margin-left: 3rem;
  margin-top: 0.1rem;
}

.comments-list {
  margin-bottom: 1rem;
  margin-left: 0.5rem;
  max-width: 100%;
}

.comment-item {
  max-width: 90%;
  margin-left: 0.5rem;
  font-size: small;
  margin-top: 0.25rem;
  margin-bottom: 0.5rem;
}

.flex-row {
  display: flex;
  flex-direction: row;
}

.comment-author {
  font-weight: bold;
}

.comment-timestamp {
  margin-left: 0.25rem;
}

.comment-content {
  margin-left: 1rem;
  margin-top: 0.15rem;
  word-wrap: break-word;
  overflow-wrap: break-word;
  word-break: break-all;
  max-width: 100%;
}

.comment-input {
  border-color: var(--main-color-primary);
  color: var(--main-color-primary);
  margin-left: 3rem;
  border-style: solid;
  border-width: 0.01rem;
  border-radius: 0.2rem;
}

.comment-send-button {
  color: var(--color-primary);
}
</style>