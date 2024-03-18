<template>
  <div class="topic">
    <h1>Helfende Hände</h1>
    <ul class="list">
      <li v-for="(item, index) in supportTasks" :key="item.title" class="card_scroll_container">
        <div :class="{card_success: item.supporterUserNames.length === item.requiredSupporters}" class="card">
          <div class="card_content">
            <h3 :class="{}">{{ item.title }}</h3>
            <p class="">{{ item.description }}</p>
            <p class="">{{ item.duration + "no" }}</p>
            <div class="progress_bar">
              <div class="progress_bar_shell">
                <span :style="{width: 100 * item.supporterUserNames.length / item.requiredSupporters + '%'}"
                      class="progress_bar_progress">
                  <p :class="{progress_bar_progress_empty: item.supporterUserNames.length === 0}">{{
                      "" + item.supporterUserNames.length.toString() + "/" + item.requiredSupporters.toString()
                    }} </p>
                </span>
              </div>
            </div>
          </div>
          <div class="card_action_button_container">
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
    handleScroll: function () {
      this.isSticky = window.scrollY > 750;
    },
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
  flex-direction: row;
  width: 100%;
  height: 100%;
  min-height: 3rem;
  padding-left: 0.5rem;
  padding-top: 0.5rem;
  margin-bottom: 0.5rem;
  border: 0.1rem solid var(--main-color-primary);
  border-radius: 0.1rem;
}

.card_success {
}

.card_content {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  align-content: center;
  border-left: .25rem solid var(--color-background);
  margin-right: auto;
  flex-basis: 75%;
  width: 100%;
  justify-content: space-between;
}

.card_action_button_container {
  height: 100%;
  margin-right: 0.25rem;
  margin-bottom: 0.25rem;
  margin-top: auto;
}

.card_action_button {
  padding: 0.25rem;
  width: 100%;
  min-width: 5rem;
  height: 100%;
  border: none;
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
  height: 2rem;
  display: flex;
  justify-content: center;
  align-items: center;
  flex-shrink: 4;
  position: relative;
  padding: 0.5rem;

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
  left: 2rem;
}

</style>
<style scoped src="src/assets/topics.css"></style>
<style scoped src="src/assets/instructions.css">
</style>