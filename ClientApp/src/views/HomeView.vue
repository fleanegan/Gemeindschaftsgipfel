<template>
  <div class="title">
    <p>Gemeinschaftsgipfel<span style="display: block; margin-bottom: 0.5rem;">2025</span></p>
    <img v-if="isNotScrolled" alt="arrow" class="animate_scroll_down_motivator" src="/down.svg" style="width: 2rem">
  </div>
  <div id="action_container" class="action_container">
    <img alt="Icon" src="/icon.svg" style="width: 100%; height:100%;"/>
  </div>
  <div :class="{'slider':true, 'key_information_slider': true}"></div>
  <info-box :data="secretData" :globalProgress="progress.infoScreen"></info-box>
  <div class="content_box">
    <div class="text_paragraph" v-html="secretData.content"></div>
    <router-link :class="{'topic_call_to_action': true, 'call_to_action_highlight': true, 'topic_call_to_action_animation': currentScreen > 1.5}"
                 to="/topic">Thema einreichen
    </router-link>
    <router-link :class="{'topic_call_to_action': true, 'call_to_action_highlight': true, 'topic_call_to_action_animation1': currentScreen > 1.5}"
                 to="/supporttask">Arbeitsgruppe beitreten
    </router-link>
  </div>
  <div :class="{'slider':true, 'key_information_slider': false}"></div>
</template>

<script lang="ts">
import {defineComponent} from 'vue';
import InfoBox from "@/views/InfoBox.vue";
import axios from "axios";

export default defineComponent({
  components: {InfoBox},
  data() {
    return {
      actionContainer: document.getElementById('action_container'),
      actionLimits: {xMin: 0, yMin: 0, xMax: 0, yMax: 0, maxLength: 0, minLength: 0, scrollPixelForAnimation: 0},
      progress: {infoScreen: 0, secondScreen: 0, thirdScreen: 0, fourthScreen: 0},
      currentScreen: 0,
      px2rem: 1 / 16,
      isNotScrolled: true,
      secretData: {} as any,
    };
  },
  methods: {
    moveTransformActionContainer(s: number) {
      this.actionContainer!.style.top = (1 - s) * this.actionLimits.yMax + this.actionLimits.yMin + "rem";
      const newLeft = (1 - s) * this.actionLimits.xMax;
      this.actionContainer!.style.left = (newLeft > this.actionLimits.xMin ? newLeft : this.actionLimits.xMin) + "rem";
      this.actionContainer!.style.width = this.actionLimits.minLength + (1 - s) * this.actionLimits.maxLength + "rem";
      this.actionContainer!.style.height = this.actionLimits.minLength + (1 - s) * this.actionLimits.maxLength + "rem";
    }, handleScroll() {
      const s = scrollY > this.actionLimits.scrollPixelForAnimation ? 1 : scrollY / this.actionLimits.scrollPixelForAnimation;
      this.updateCurrentScreenNumber()
      this.moveTransformActionContainer(s);
      this.isNotScrolled = scrollY < 1;
      this.progress.infoScreen = this.updateInfoScreenProgress(0, 1.5)
      this.progress.secondScreen = this.updateInfoScreenProgress(1.4, 1.5)
      this.progress.thirdScreen = this.updateInfoScreenProgress(1.5, 1.6)
      this.progress.fourthScreen = this.updateInfoScreenProgress(1.7, 1.8)
      this.updateActionLimits()
    },
    updateCurrentScreenNumber() {
      this.currentScreen = scrollY / window.screen.availHeight
    },
    handleResize() {
      this.updateActionLimits()
      this.handleScroll()
    },
    updateActionLimits() {
      this.actionLimits.xMax = (window.innerWidth - this.actionContainer!.offsetWidth) / 2 * this.px2rem;
    },
    setUpActionContainer() {
      this.actionLimits.maxLength = 125 * this.px2rem;
      this.actionLimits.minLength = 50 * this.px2rem;
      this.actionContainer = document.getElementById('action_container')!;
      this.actionLimits.scrollPixelForAnimation = 15 / this.px2rem;
      const offsetLeft = this.actionContainer.getBoundingClientRect().x
      this.actionLimits.xMin = offsetLeft * this.px2rem;
      this.updateActionLimits()
      this.actionLimits.yMin = this.actionContainer!.offsetTop * this.px2rem;
      this.actionLimits.yMax = 175 * this.px2rem;
    },
    updateInfoScreenProgress(start: number, end: number): number {
      if (this.currentScreen < start)
        return 0
      else if (this.currentScreen > end)
        return 1
      return (this.currentScreen - start) / (end - start)
    },
  },
  async mounted() {
    this.secretData = (await axios.get('/api/home/getinfo', {})).data
    this.px2rem = 1 / parseFloat(getComputedStyle(document.documentElement).fontSize)
    this.setUpActionContainer();
    this.updateCurrentScreenNumber()
    this.handleResize()
    this.moveTransformActionContainer(0);
    window.addEventListener('scroll', this.handleScroll);
    window.addEventListener('resize', this.handleResize);
  },
  unmounted() {
    window.removeEventListener('scroll', this.handleScroll);
    window.removeEventListener('resize', this.handleResize);
  },
});
</script>

<style scoped src="src/assets/main.css"></style>
<style scoped>

.title {
  display: flex;
  flex-direction: column;
  align-items: center;
  min-height: 100vh;
  height: 100vh;
  z-index: 7;
}

.title p {
  margin-top: 25rem;
  font-size: 2rem;
}

.title img {
  margin-top: auto;
  margin-bottom: 0.5rem;
}

.action_container {
  width: 5rem;
  height: 5rem;
  position: fixed;
  left: 0.575rem;
  z-index: 9999;
}

.animate_scroll_down_motivator {
  position: absolute;
  bottom: 0;
  animation: fadeInOut 1s infinite alternate ease-in-out; /* Animation name, duration, iteration count, and direction */
}

@keyframes fadeInOut {
  0% {
    opacity: 1; /* Start with opacity 0 */
    transform: translateY(0.25rem);
  }
  100% {
    opacity: 0.6; /* Fade in to opacity 1 */
    transform: translateY(-0.25rem);
  }
}

.slider {
  min-height: 100vh;
  min-width: 100%;
}

.key_information_slider {
  background-color: var(--main-color-primary);
  min-height: 105vh;
}

.topic_call_to_action {
  margin-left: auto;
  margin-right: auto;
  border: 0.1rem solid var(--main-color-primary);
  border-radius: 0.2rem;
  width: 15rem;
  display: flex;
  place-content: center;
  margin-bottom: 0.5rem;
  margin-top: 0.5rem;
}

.call_to_action_highlight{
  background-color: var(--main-color-secondary);
  color: white;
  border: none;
}

.call_to_action_highlight:hover {
background-color: var(--main-color-primary);
}

.topic_call_to_action_animation {
  animation: bounce 1s ease infinite;
}

@keyframes bounce {
  0%, 20%, 50%, 80%, 100% {
    transform: translateY(0);
  }
  40% {
    transform: translateY(-10px);
  }
  60% {
    transform: translateY(-5px);
  }
}

.topic_call_to_action_animation1 {
  animation: bounceDown 1s ease infinite;
}

@keyframes bounceDown {
  0%, 20%, 50%, 80%, 100% {
    transform: translateY(0);
  }
  40% {
    transform: translateY(5px);

  }
  60% {
    transform: translateY(10px);

  }
}

.content_box {
  margin-right: auto; 
  margin-left: auto;
  z-index: 10;
}

:deep(h1) {
  margin: 0rem 0rem 1rem 0rem;
  padding: 0;
}
:deep(h2) {
  margin: 0rem 0rem 0.5rem 0rem;
  padding: 0;
}

:deep(br) {
  margin-bottom: 2rem;
}

:deep(ul) {
  margin-top: 0.5rem;
  list-style-position: outside;
}

:deep(li) {
  margin-bottom: 0.5rem;
}

:deep(li p) {
  margin-left: 0.75rem;
}

:deep(li h3) {
  margin-left: 0.75rem;
}

.text_paragraph {
  margin: 2rem 2rem 0 2rem;
  max-width: 45rem;

}

.title p {
  margin-top: 25rem;
  font-size: 2rem;
  text-align: center;
}

</style>
