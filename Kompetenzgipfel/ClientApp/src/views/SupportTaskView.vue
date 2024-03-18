<template>
  <div class="topic">
    <h1>Helfende Hände</h1>
    <p class="descriptiona">Bittebitte, hilfehilfe. Hier kannst du dich eintragen, um bei Aufgaben mitzuhelfen, bei denen wir Tatkraft
      brauchen!</p>
    <ul class="list">
      <li v-for="(item, index) in supportTasks" :key="item.title" class="card_scroll_container">
        <div :class="{card_success: item.supporterUserNames.length === item.requiredSupporters}" class="card">
          <div class="card_content">
            <h3 class="support_task_header">{{ item.title }}</h3>
            <p class="">{{ item.description }}</p>
            <p class="support_task_duration">{{ item.duration }}</p>
          </div>
          <div class="card_action_button_container">
            <div class="progress_bar">
              <div class="progress_bar_shell">
                <span :style="{width: calcProgressBarWidth(item)}"
                      class="progress_bar_progress">
                  <p :class="{progress_bar_progress_empty: item.supporterUserNames.length === 0}">{{
                      "" + item.supporterUserNames.length.toString() + "/" + item.requiredSupporters.toString()
                    }} </p>
                </span>
              </div>
            </div>
            <button
                :class="{card_action_button: true, card_action_button_active: item.supporterUserNames.includes(userName!), card_action_button_inactive: !item.supporterUserNames.includes(userName!)}"
                @click="toggleSupporting(index)">
              {{ item.supporterUserNames.includes(userName!) ? "Austragen" : "Eintragen" }}
            </button>
          </div>
        </div>
      </li>
    </ul>
  </div>
</template>


<script lang="ts">
import {defineComponent} from 'vue';
import axios from "axios";
import {useAuthStore} from '@/store/auth';

interface SupportTask {
  id: string;
  title: string;
  description: string;
  supporterUserNames: string[];
  duration: string;
  requiredSupporters: number;
}


export default defineComponent({
  data() {
    return {
      supportTasks: [] as SupportTask[],
      isSticky: false,
    };
  },
  methods: {
    useAuthStore,
    async fetchData() {
      this.supportTasks = (await axios.get('/api/supporttask/getall', {})).data;
    },
    async toggleSupporting(index: number): Promise<void> {
      if (this.supportTasks[index].supporterUserNames.includes(this.userName!)) {
        try {
          await axios.delete('/api/supporttask/help/' + this.supportTasks[index].id)
        } catch (e: any) {
          console.log(e.status, e.response)
        }
        this.supportTasks[index].supporterUserNames = this.supportTasks[index].supporterUserNames.filter(x => x != this.userName!)
      } else {
        try {
          await axios.post('/api/supporttask/help', {"SupportTaskId": this.supportTasks[index].id})
        } catch (e: any) {
          console.log(e.status, e.response)
        }
        this.supportTasks[index].supporterUserNames.push(this.userName!)
      }
    },
    calcProgressBarWidth(item: SupportTask) {
      if (item.requiredSupporters === 0)
        return 0;
      const normalWidth = item.supporterUserNames.length / item.requiredSupporters;
      let result = '';
      if (normalWidth < 0.075 && item.supporterUserNames.includes(this.userName!)) {
        result = "10%";
      } else {
        result = 100 * normalWidth + "%";
      }
      return result;
    },
    handleScroll: function () {
      this.isSticky = window.scrollY > 750;
    }
    ,
  },
  computed: {
    userName() {
      return this.useAuthStore().userName;
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

.card_success {
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
}

.card_action_button_container {
  width: 100%;
  margin-right: 0.25rem;
  margin-bottom: 0.25rem;
  margin-top: auto;
  display: flex;
  flex-direction: column;
  padding: 0.5rem;
}

.card_action_button {
  padding: 0.25rem;
  margin-left: auto;
  margin-right: 0;
  min-width: 5rem;
  height: 1.5rem;
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
  font-size: large;
}

.support_task_duration {
  margin-left: auto;
  margin-top: 0.5rem;
  margin-right: 0.5rem;
}

.descriptiona {
  font-size: 1.25rem;
  margin: 1rem;
  margin-right: 2rem;
  margin-bottom: 5rem;
}

</style>
<style scoped src="src/assets/topics.css"></style>
<style scoped src="src/assets/instructions.css">
</style>