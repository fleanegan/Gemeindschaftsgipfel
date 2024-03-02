<template>
  <header :class="{'nav_header': true, 'nav_header_sticky': isSticky}">
    <nav class="nav-links">
      <router-link class="router-link" to="/"><img alt="Home" src="/public/icon.svg"
                                                   style="width: 6rem; height: 6rem; max-height: 32px; max-width: 32px;">
      </router-link>
      <div class="transparent-header-area"></div>
      <router-link class="router-link" to="/about">About</router-link>
      <router-link class="router-link" to="/login">+</router-link>
      <router-link class="router-link" to="/topic">Vortragsthemen</router-link>
    </nav>
  </header>
  <div class="routed-elements">
    <router-view/>
  </div>
</template>

<script lang="ts">
import {defineComponent} from 'vue';

export default defineComponent({
  data() {
    return {
      isSticky: false,
    };
  },
  methods: {
    handleScroll: function () {
      this.isSticky = window.scrollY > 0;
    },
  },
  mounted() {
    window.addEventListener('scroll', this.handleScroll);
  },
  beforeUnmount() {
    window.removeEventListener('scroll', this.handleScroll);
  },
});
</script>

<style scoped>
.nav_header {
  width: 100%;
  position: fixed;
  overflow-anchor: none;
  top: 0;
  left: 0;
  z-index: 999;
  background-color: rgba(255, 255, 255, 0);
  pointer-events: none;
  max-height: 10px;
}

@media (max-width: 900px) {
  .nav_header_sticky {
    background-color: rgba(255, 255, 255, 90);
    transition: background-color 0.6s ease;
  }
}

.router-link {
  pointer-events: auto;
}

.transparent-header-area {
  margin-left: auto;
  margin-right: auto;
  pointer-events: none;
}

.nav-links {
  display: flex;
  justify-content: flex-start;
  flex-direction: row;
  align-items: center;
  width: 100%;
  padding: 0 1rem 0.5rem 0.5rem;
}


.routed-elements {
  margin-top: 5rem;
  width: 500px;
  max-width: 900px;
  height: 100vh;
  display: flex;
  flex-direction: column;
  padding-left: 0.5rem;
}

@media (max-width: 900px) {
  .routed-elements {
    width: 100%;
  }
}
</style>
