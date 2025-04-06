<!-- GlobalLoading.vue -->
<template>
  <div v-if="showLoading" class="global-loading-overlay">
    <img alt="arrow" class="animate_scroll_down_motivator" src="/loading.webp" style="width: 40%; max-width:350px;">
    <h1 style='padding:1.25rem;'>Loading...</h1>
  </div>
</template>

<script>
import {useDataStore} from '@/store/data';
import {storeToRefs} from 'pinia';

export default {
  name: 'GlobalLoadingView',
  setup() {
    const dataStore = useDataStore();
    const {isLoading} = storeToRefs(dataStore);
    return {isLoading};
  },
  data() {
    return {
      showLoading: false,
      loadingTimeout: null,
      delay: 400
    };
  },
  watch: {
    isLoading(newVal) {
      if (newVal) {
        if (this.loadingTimeout) clearTimeout(this.loadingTimeout);
        this.loadingTimeout = setTimeout(() => {
          if (this.isLoading) {
            this.showLoading = true;
          }
        }, this.delay);
      } else {
        if (this.loadingTimeout) clearTimeout(this.loadingTimeout);
        this.showLoading = false;
      }
    }
  }
}
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
