<template>
  <div class="seperator"></div>
  <div id="action_container" class="action_container">
    <img alt="Icon" src="/public/icon.svg" style="width: 100%; height:100%;"></img>
  </div>
  <div v-if="currentScreen < -.25" class="slide">
    <p class="slide_header">Gemeindschaftsgipfel</p>
  </div>
  <div v-if="currentScreen > 0 && currentScreen < 0.5" class="slide">
    <div class="slide_content">
      <h2>Geimeindschaft</h2>
      <p>Alle in einem Boot, aber ohne Boot. Alle an einem Strang ziehen, ohne jemandem einen Strick draus zu
        drehen. </p>
    </div>
  </div>
  <div v-if="currentScreen > 1 && currentScreen < 1.5" class="slide">
    <div class="slide_content">
      <h2>Gipfel</h2>
      "Das ist ja die Hoehe!"
      <br>
      - Tante Ulrike
      <br>
      <br>
      <br>
      <p>Hier wächst zusammen, was zusammen gehört. Wir. Zusammen. Miteinander.
      </p>
    </div>
  </div>
  <div v-if="currentScreen > 2 && currentScreen < 2.5" class="slide">
    <div class="slide_content">
      <h2>Alle wichtigen Informationen</h2>
      <h3>Wann findet statt?</h3>
      <h3>Wo findet statt?</h3>
      <h3>Wen anrufen, wenn Problem?</h3>
    </div>
  </div>
  <div v-if="currentScreen > 3" class="slide">
    <div class="slide_content">
      <h2>Da will ich mitmachen!</h2>
      <h1></h1>
      <router-link class="router-link" to="/topic">Jetzt Thema einreichen</router-link>
    </div>
  </div>
  <br>
  <div class="seperator" style="background-color: #f66900"></div>
  <div class="seperator" style="background-color: #6f6f6f"></div>
  <div class="seperator" style="background-color: #13b7fe"></div>
  <div class="seperator" style="background-color: #f1e5b6"></div>
  <div class="seperator" style="background-color: #c5b57b"></div>
  <div class="seperator" style="background-color: #d8c680"></div>
  <div class="seperator" style="background-color: #f8e7a3"></div>
  <div class="seperator" style="background-color: #fae490"></div>
  <div class="seperator" style="background-color: #ddca7c"></div>
</template>


<script lang="ts">
import {defineComponent} from 'vue';

export default defineComponent({
  data() {
    return {
      actionContainer: document.getElementById('action_container'),
      actionLimits: {xMin: 0, yMin: 0, xMax: 0, yMax: 0, maxLength: 0, minLength: 0, scrollPixelForAnimation: 0},
      currentScreen: 0,
      px2rem: 1 / 17.666666666667,
    };
  },
  methods: {
    moveTransformActionContainer(s: number) {
      this.actionContainer!.style.top = (1 - s) * this.actionLimits.yMax + this.actionLimits.yMin + "rem";
      this.actionContainer!.style.left = (1 - s) * this.actionLimits.xMax + this.actionLimits.xMin + "rem";
      this.actionContainer!.style.width = this.actionLimits.minLength + (1 - s) * this.actionLimits.maxLength + "rem";
      this.actionContainer!.style.height = this.actionLimits.minLength + (1 - s) * this.actionLimits.maxLength + "rem";
    }, handleScroll() {
      const s = scrollY > this.actionLimits.scrollPixelForAnimation ? 1 : scrollY / this.actionLimits.scrollPixelForAnimation;
      this.updateCurrentScreenNumber()
      this.moveTransformActionContainer(s);
    },
    updateCurrentScreenNumber() {
      this.currentScreen = scrollY / window.screen.availHeight - 0.5
    }
  },
  computed: {},
  mounted() {
    this.actionContainer = document.getElementById('action_container')!;
    this.actionLimits.scrollPixelForAnimation = 15 / this.px2rem;
    this.actionLimits.xMin = this.actionContainer!.offsetLeft * this.px2rem;
    this.actionLimits.xMax = (window.innerWidth - this.actionContainer!.offsetWidth) / 2 * this.px2rem;
    this.actionLimits.yMin = this.actionContainer!.offsetTop * this.px2rem;
    this.actionLimits.yMax = 175 * this.px2rem;
    this.actionLimits.maxLength = 125 * this.px2rem;
    this.actionLimits.minLength = 50 * this.px2rem;
    this.moveTransformActionContainer(0);
    this.updateCurrentScreenNumber()
    window.addEventListener('scroll', this.handleScroll);
  },
  unmounted() {
    window.removeEventListener('scroll', this.handleScroll);
  },
});
</script>

<style scoped>

.action_container {
  width: 5rem;
  height: 5rem;
  position: fixed;
  top: 0.15rem;
  left: 0.5rem;
  z-index: 9999;
}

.seperator {
  min-height: 100vh;
  max-width: 10px;
}

.slide {
  position: fixed;
  height: 100vh;
  min-height: 100vh;
  width: 100%;
  min-width: 350px;
  z-index: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  left: -0rem;
  padding: 2rem;
}

.slide_header {
  font-size: 2rem;
  position: relative;
  top: 22rem;
}

.slide_content {
  font-size: 1rem;
  position: relative;
  top: 10rem;
  margin-left: 2rem;
  width: 25rem;
  min-width: 350px;
}

.slide_content h2 {
  margin: 0 0 2rem;
}

</style>