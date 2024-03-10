<template>
  <div style="min-height: 90vh"></div>
  <div id="action_container" class="action_container">
    <img alt="Icon" src="/icon.svg" style="width: 100%; height:100%;"></img>
  </div>
  <div v-show="currentScreen < 0" class="info_tile">
    <p class="info_tile_header">Gemeindschaftsgipfel</p>
  </div>
  <div v-show="currentScreen > 0 && currentScreen < 1" class="info_tile">
    <div class="info_tile_content">
      <div class="key_information">
        <h2>Das Wichtigste auf einen Blick</h2>
        <div class="key_information_detail">
          <img alt="map" class="key_information_detail_image" src="/map.svg">
          <p>Koordinaten</p>
        </div>
        <div class="key_information_detail">
          <img alt="calendar" class="key_information_detail_image" src="/calendar.svg">
          <p>28.06.2024 - 30.06.2024</p>
        </div>
        <div class="key_information_detail">
          <img alt="housing" class="key_information_detail_image" src="/housing.svg">
          <p>Bring dein Zelt mit!</p>
        </div>
        <div class="key_information_detail">
          <img alt="wifi" class="key_information_detail_image" src="/wifi.svg">
          <table>
            <tr>
              <td>SSID:</td>
              <td>sich3rh4it</td>
            </tr>
            <tr>
              <td>Password:</td>
              <td>123456</td>
            </tr>
          </table>
        </div>
      </div>
    </div>
  </div>
  <div v-show="currentScreen > 1 && currentScreen < 1.5" :class="{'info_tile':true}">
    <div :class="{'info_tile_content':true, 'slide-in':true}">
      <h2>Geimeindschaft</h2>
      <p>Alle in einem Boot, aber ohne Boot. Alle an einem Strang ziehen, ohne jemandem einen Strick draus zu
        drehen. </p>
    </div>
  </div>
  <div v-show="currentScreen > 2 && currentScreen < 2.5" class="info_tile">
    <div class="info_tile_content">
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
  <div v-show="currentScreen > 3" class="info_tile">
    <div class="info_tile_content">
      <h2>Da will ich mitmachen!</h2>
      <h1></h1>
      <router-link class="animated" to="/topic">Jetzt Thema einreichen</router-link>
    </div>
  </div>
  <br>
  <div :class="{'slider':true, 'key_information_slider': true}"></div>
  <div class="slider"></div>
  <div class="slider"></div>
  <div class="slider"></div>
  <div class="slider"></div>
  <div class="slider"></div>
  <div class="slider"></div>
  <div class="slider"></div>
  <div class="slider"></div>
</template>


<script lang="ts">
import {defineComponent} from 'vue';

export default defineComponent({
  data() {
    return {
      actionContainer: document.getElementById('action_container'),
      actionLimits: {xMin: 0, yMin: 0, xMax: 0, yMax: 0, maxLength: 0, minLength: 0, scrollPixelForAnimation: 0},
      currentScreen: 0,
      px2rem: 1 / 16,
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
    },
    updateCurrentScreenNumber() {
      this.currentScreen = scrollY / window.screen.availHeight - 0.5
    },
    handleResize() {
      this.handleScroll()
      this.updateActionLimits()
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
    }
  },
  computed: {},
  mounted() {
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

<style scoped>

.action_container {
  width: 5rem;
  height: 5rem;
  position: fixed;
  left: 0.575rem;
  z-index: 99999;
}

.slider {
  min-height: 100vh;
  min-width: 100%;
}

.key_information {
  color: var(--color-background);
  border: 0.2rem solid var(--color-background);
  padding: 1rem;
}

.key_information_detail {
  display: flex;
  flex-direction: row;
  width: 100%;
  margin: 1rem;
  align-items: center;
}

.key_information_detail p {
  margin-right: auto;
}

.key_information_detail_image {
  margin-right: 1rem;
  width: 3rem;
  height: 3rem;
}

.info_tile {
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

.key_information_slider {
  background-color: var(--main-color-primary);
  min-height: 105vh;
}


.info_tile_header {
  font-size: 2rem;
  position: relative;
  top: 22rem;
}

.info_tile_content {
  font-size: 1rem;
  position: relative;
  top: 25%;
  margin-left: 2rem;
  width: 25rem;
  min-width: 350px;
}

.info_tile_content h2 {
  margin: 0 0 2rem;
}

.animated {
  /* Define your animation using CSS keyframes */
  animation: bounce 1s ease infinite; /* Adjust the duration and easing as needed */
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

table {
  border-collapse: collapse;
  width: 100%;
  margin-right: 2rem;
  margin-left: -0.5rem;
}

th, td {
  border: none;
  padding: 8px;
  text-align: left;
}

/* Apply specific width to the first column */
th:first-child, td:first-child {
  width: 70%;
}


.show-enter-active,
.show-leave-enter {
  transform: translateX(-20%);
  transition: all 3s linear;
}

.show-enter-to {
  transform: translateX(0%);
  transition: all .3s linear;
}
</style>