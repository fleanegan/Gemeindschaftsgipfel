<template>
  <div class="topic">
    <h1>Helfende Hände</h1>
    <ul class="list">
      <li v-for="(item, index) in supportTasks" :key="item.title" class="card_scroll_container">
        <div class="card">
          <div class="card_content">
            <h3 :class="{}">{{ item.title }}</h3>
            <p class="">{{ item.description }}</p>
          </div>
          <div class="card_action_button_container">
            <button class="card_action_button" @click="toggleSupporting(index)">
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
        await axios.delete('/api/supporttask/help/' + this.supportTasks[index].id)
        this.supportTasks[index].supporterUserNames = this.supportTasks[index].supporterUserNames.filter(x => x != this.userName!)
      } else {
        await axios.post('/api/supporttask/help', {"SupportTaskId": this.supportTasks[index].id})
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
  background-color: aliceblue;
  padding-left: 0.5rem;
  padding-top: 0.5rem;
  margin-bottom: 0.5rem;
}

.card_content {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  align-content: center;
  border-left: .25rem solid var(--color-background);
  background-color: antiquewhite;
  margin-right: auto;
}

.card_action_button_container {
  height: 100%;
  margin-right: 0.25rem;
  margin-bottom: 0.25rem;
  margin-top: auto;
  background-color: #e8e8e8;
}

.card_action_button {
  padding: 0.25rem;
  width: 100%;
  height: 100%;
  border: none;
  border-radius: 0.2rem;
}

</style>
<style scoped src="src/assets/topics.css"></style>
<style scoped src="src/assets/instructions.css">
</style>