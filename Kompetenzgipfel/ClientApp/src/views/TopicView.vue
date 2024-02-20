<template>
  <div class="about">
    <h1>This page contains protected content:</h1>
    <br>
    <p>{{ protectedContent }}</p>
  </div>
</template>


<script lang="ts">
import {defineComponent, onMounted, ref} from 'vue';
import axios from 'axios';

export default defineComponent({
  setup() {
    // Define a reactive reference to store the fetched data
    const protectedContent = ref([]);

    onMounted(async () => {
      try {
        const response = await axios.get('/api/protected'); // Replace with your API endpoint
        protectedContent.value = response.data;
      } catch (error) {
        console.error('Error fetching data:', error);
      }
    });

    // Return the reactive data to be used in the template
    return {
      protectedContent,
    };
  },
});
</script>
