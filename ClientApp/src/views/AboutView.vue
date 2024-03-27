<template>
  <h1>Template</h1>
  <button @click="callRejector()">get rejected</button>
</template>


<script lang="ts">
import {defineComponent} from 'vue';
import axios from "axios";

export default defineComponent({
  data() {
    return {};
  },
  methods: {
    async callRejector() {
      await axios.get('/api/auth/rejectme', {});
    }
  },
  computed: {}
});
</script>

