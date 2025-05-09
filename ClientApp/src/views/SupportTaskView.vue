<template>
  <div class="topic">
    <h1>Helfende Hände</h1>
    <p class="support_description">Freiwillige vor! Wir haben ein paar Aufgaben gesammelt, bei denen wir Hilfe brauchen.
      Mach
      mit und schaff die letzten Hürden auf dem Weg zum Gemeinschaftsgipfel aus dem Weg. Keine Scheu, hier steht das
      Vergnügen proportional zum Schweiß : Jeder Dienst wird in Dreiergruppen gestaltet, damit du auch bei diesem Teil
      des Festivals immer von netten Menschen umgeben bist. Zur vergeben sind (oh ja, du darfst dich auch mehrmals
      eintragen):</p>
    <h2>Gruppenaufgaben</h2>
    <p class="support_description">Hier zu helfen kostet nicht so viel Zeit und Energie, hilft aber ungemein</p>
    <ul class="list">
      <li v-for="(item, index) in groupSupportTasks" :key="index" class="card_scroll_container">
        <SupportTaskCard
          :task="item"
          :taskList="groupSupportTasks"
          :taskIndex="index"
          :toggleSupporting="toggleSupporting"
          :user-name="userName!"
        />
      </li>
    </ul>
    <h2><span style="font-weight: bold">NEU!</span> Hauptverantwortliche</h2>
    <p class="support_description">Hierfür braucht es etwas Engagement, und der Ruhm wird ewig währen.
      Zu Anfang des Festivals werden wir dich in Alles einweihen, was du wissen musst, damit du den spannenden Aufgaben
      problemlos Herr wirst und dann in der Lage bist, alle anderen Teilnehmer:innen in allen Fragen rund um deine Verantwortlichkeit zu unterstützen.
    </p>
    <ul class="list">
      <li v-for="(item, index) in singleSupportTasks" :key="index" class="card_scroll_container">
        <SupportTaskCard
          :task="item"
          :taskList="singleSupportTasks"
          :taskIndex="index"
          :toggleSupporting="toggleSupporting"
          :user-name="userName!"
        />
      </li>
    </ul>
    <div style="color: var(--main-color-secondary); margin: 1.5rem 0 0 1rem; font-size: small">{{allTheTaskNamesISubscribedTo.length ? "Da unterstützt du schon: " : " " }}</div>
    <div style="background: var(--main-color-secondary); border: var(--main-color-secondary) .2rem solid; border-radius: .1rem; padding: 0 1rem 0 1rem; margin: .25rem 1.5rem 1rem 1rem; color: white">
      {{allTheTaskNamesISubscribedTo.length? allTheTaskNamesISubscribedTo.join(",") : "Oha, du hast dich noch nirgendwo eingetragen"}}
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import axios from "axios";
import { useAuthStore } from '@/store/auth';
import SupportTaskCard from './SupportTaskCard.vue';

export interface SupportTask {
  id: string;
  title: string;
  description: string;
  supporterUserNames: string[];
  duration: string;
  requiredSupporters: number;
  showSupporter: boolean;
}

export default defineComponent({
  components: {
    SupportTaskCard
  },
  data() {
    return {
      supportTasks: [] as SupportTask[],
      isSticky: false,
    };
  },
  methods: {
    useAuthStore,
    async fetchData() {
      this.supportTasks = (await axios.get('/api/supporttask/getall', {})).data.sort(function (a: SupportTask, b: SupportTask) {
        if (a.duration > b.duration)
          return 1;
        if (a.duration < b.duration)
          return -1;
        return 0;
      });
    },
    async toggleSupporting(tasks: SupportTask[], index: number): Promise<void> {
      if (tasks[index].supporterUserNames.includes(this.userName!)) {
        try {
          await axios.delete('/api/supporttask/help/' + tasks[index].id)
        } catch (e: any) {
          console.log(e.status, e.response)
        }
        tasks[index].supporterUserNames = tasks[index].supporterUserNames.filter(x => x != this.userName!)
      } else {
        try {
          await axios.post('/api/supporttask/help', {"SupportTaskId": tasks[index].id})
        } catch (e: any) {
          console.log(e.status, e.response)
        }
        tasks[index].supporterUserNames.push(this.userName!)
      }
    },
    handleScroll: function () {
      this.isSticky = window.scrollY > 750;
    },
  },
  computed: {
    userName() {
      return this.useAuthStore().userName;
    },
    groupSupportTasks() {
      return this.supportTasks.filter((task: SupportTask) => task.requiredSupporters > 1);
    },
    singleSupportTasks() {
      return this.supportTasks.filter((task: SupportTask) => task.requiredSupporters === 1);
    },
    allTheTaskNamesISubscribedTo() {
      return this.supportTasks
          .filter(task => task.supporterUserNames.includes(this.userName!))
          .map(task => task.title)
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
.support_description {
  margin: 1rem 2rem 1rem 1rem;
}

h2 {
  margin-top: 3rem;
}

</style>
<style scoped src="src/assets/topics.css"></style>
<style scoped src="src/assets/instructions.css">
</style>
