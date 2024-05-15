<!-- GlobalLoading.vue -->
<template>
  <div v-if="isLoading" class="global-loading-overlay">
<img alt="arrow" class="animate_scroll_down_motivator" src="/loading.webp" style="width: 40%; max-width:350px;">
<h1 style='padding:1.25rem;'>Loading...</h1>
  </div>
</template>

<script lang="ts">
import {defineComponent} from 'vue';
import {useDataStore} from '@/store/data';
import {useAuthStore} from '@/store/auth';

export default defineComponent({
  data() {
    return {
      dummy : '',
      content : 'connection to pinia not yet established',
    };
  },
    computed: {
	    isLoading(){return useDataStore().isLoading}
    },
    mounted() {
    },
})
</script>

<style scoped>
.global-loading-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(255, 255, 255, 0.9);
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  z-index: 9999; 
}
</style>
