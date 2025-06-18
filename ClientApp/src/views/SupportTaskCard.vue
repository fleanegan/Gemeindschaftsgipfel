<template>
  <div class="card">
    <div class="card_content">
      <h3 class="support_task_header">{{ task.title }}</h3>
      <p>{{ task.description }}</p>
      <p class="support_task_duration">{{ task.duration }}</p>
    </div>
    <div class="card_action_container">
      <div class="progress_bar">
        <div class="progress_bar_shell">
          <span :style="{width: calcProgressBarWidth(task)}" class="progress_bar_progress">
            <p :class="{progress_bar_progress_empty: task.supporterUserNames.length === 0}">
              {{ task.supporterUserNames.length + '/' + task.requiredSupporters }}
            </p>
          </span>
        </div>
      </div>
      <div class="card_action_button_container">
        <div @mouseenter="handleMouseEnter" @mouseleave="handleMouseLeave">
          <img v-if="isUserSubscribed(task)" src="/helper_filled.svg" alt="helper">
          <img v-else src="/helper.svg" alt="helper">
        </div>
        <div class="card_action_helper_list" v-if="task.showSupporter">
          <p>Wir helfen schon:</p>
          <div v-for="supporter in task.supporterUserNames" :key="supporter">
            <p style="margin-left: 0.5rem; font-size: 0.75rem">{{ supporter }}</p>
          </div>
        </div>
        <p v-if="isUserSubscribed(task) && !task.showSupporter" class="card_action_helper_list">Ich habe mich
          eingetragen</p>
        <button
            :class="{
            card_action_button: true,
            card_action_button_active: isUserSubscribed(task),
            card_action_button_inactive: !isUserSubscribed(task)
          }"
            @click="toggleSupporting(taskList, taskIndex)">
          {{ isUserSubscribed(task) ? 'Austragen' : 'Eintragen' }}
        </button>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import {defineComponent} from 'vue';
import type {PropType} from 'vue';
import type {SupportTask} from '@/views/SupportTaskView.vue';

export default defineComponent({
  props: {
    task: {
      type: Object as PropType<SupportTask>,
      required: true
    },
    taskList: {
      type: Array as PropType<SupportTask[]>,
      required: true
    },
    taskIndex: {
      type: Number,
      required: true
    },
    toggleSupporting: {
      type: Function as PropType<(tasks: SupportTask[], index: number) => void>,
      required: true
    },
    userName: {
      type: String,
      required: true
    }
  },
  setup(props, {emit}) {
    const calcProgressBarWidth = (task: SupportTask): string => {
      if (task.requiredSupporters === 0)
        return '0';
      const normalWidth = task.supporterUserNames.length / task.requiredSupporters;
      let result = 0;
      if (normalWidth < 0.075 && task.supporterUserNames.includes(props.userName)) {
        result = 10;
      } else {
        result = 100 * normalWidth;
      }
      if (result > 100)
        result = 100
      return result + "%";
    };
    const isUserSubscribed = (task: SupportTask): boolean => {
      return task.supporterUserNames.includes(props.userName);
    };
    const handleMouseEnter = () :void => {
  	emit('show-supporter', true);
    }
    const handleMouseLeave = (): void => {
  	emit('hide-supporter', true);
    }
    return {
      calcProgressBarWidth,
      isUserSubscribed,
      handleMouseEnter,
      handleMouseLeave,
    };
  }
});
</script>

<style scoped>
.card {
  display: flex;
  flex-direction: column;
  width: 100%;
  height: 100%;
  min-height: 3rem;
  padding-left: 0.5rem;
  padding-top: 0.5rem;
  margin-bottom: 0.5rem;
  border-radius: 0.2rem;
  border: 0.1rem solid rgba(179, 76, 76, 0.1);
}

.card_content {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  align-content: center;
  margin-right: auto;
  flex-basis: 75%;
  width: 100%;
  justify-content: space-between;
  position: relative;
}

.card_action_container {
  width: 100%;
  margin-right: 0.25rem;
  margin-bottom: 0.25rem;
  margin-top: auto;
  display: flex;
  flex-direction: column;
  padding: 0.5rem;
}

.card_action_button_container {
  display: flex;
  flex-direction: row;
  align-content: center;
  justify-content: center;
  justify-items: center;
}

.card_action_helper_list {
  position: relative;
  margin-left: 0.4rem;
  padding-top: .2rem;
  font-size: small;
}

.card_action_button {
  padding: 0.25rem;
  margin-left: auto;
  margin-right: 0;
  min-width: 5rem;
  height: 1.75rem;
  border-radius: 0.2rem;
  border: 0.1rem solid var(--main-color-primary);
}

.card_action_button_active {
  background-color: var(--color-background);
  color: var(--main-color-primary);
}

.card_action_button_inactive {
  background-color: var(--color-primary);
  color: var(--color-background);
}


.progress_bar {
  width: 100%;
  height: 1rem;
  display: flex;
  justify-content: center;
  align-items: center;
  flex-shrink: 4;
  position: relative;
  margin-bottom: 0.75rem;
}

.progress_bar_shell {
  border: 1px solid var(--main-color-primary);
  border-radius: 0.2rem;
  width: 100%;
  height: 100%;
  font-size: 0.75rem;
}

.progress_bar_progress {
  background-color: var(--main-color-primary);
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: flex-end;
  padding-top: 0.1rem;
  padding-right: 0.5rem;
  color: var(--color-background);
}

.progress_bar_progress_empty {
  position: relative;
  color: var(--main-color-primary);
  left: 3rem;
}

.support_task_header {
  font-stretch: extra-expanded;
  font-size: 1rem;
}

.support_task_duration {
  margin-left: auto;
  margin-top: 0.5rem;
  margin-right: 0.5rem;
}
</style>
