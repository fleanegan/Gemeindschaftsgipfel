<script lang="ts">
import {defineComponent} from 'vue'

export default defineComponent({
  name: "InfoBox",
  props: ['globalProgress', 'data'],
  computed: {
    keyInformationOpacity() {
      const fadeOutStart = 0.75;
      if (this.globalProgress === 0)
        return 0
      if (this.globalProgress < fadeOutStart)
        return 1
      return (1 - 1 * this.globalProgress) / (Math.abs(1 - fadeOutStart))
    },
    borderProgress() {
      const fadeOutStart = 0.4;
      const fadeOutEnd = 0.55
      if (this.globalProgress < fadeOutStart)
        return -1
      if (this.globalProgress > fadeOutEnd)
        return 0
      const result = -1 + ((this.globalProgress - fadeOutStart)) / (fadeOutEnd - fadeOutStart)
      return result
    }
  }
})
</script>

<template>
  <div v-if="globalProgress < 1" :style="{opacity: keyInformationOpacity}" class="info_tile">
    <div class="info_tile_content">
      <svg class="frame" preserveAspectRatio="none" viewBox="29 29 142 142">
        <path
            :style="{'stroke-dashoffset': borderProgress}"
            class="path"
            d="M 30,30 L 30,170 L 170,170 L 170,30 L 30, 30"
            fill="none"
            pathLength="1"
            stroke="white"
            stroke-width="2"
        />
      </svg>
      <div class="key_information">
        <h2>{{ data.key_information_detail_title }}</h2>
        <div class="key_information_detail">
          <img alt="map" class="key_information_detail_image" src="/map.svg">
          <div v-html="data.key_information_detail_coordinates"></div>
        </div>
        <div class="key_information_detail">
          <img alt="calendar" class="key_information_detail_image" src="/calendar.svg">
          <p>{{ data.key_information_detail_dates }}</p>
        </div>
        <div class="key_information_detail">
          <img alt="housing" class="key_information_detail_image" src="/housing.svg">
          <p>{{ data.key_information_detail_accomodation }}</p>
        </div>
        <div class="key_information_detail">
          <img alt="fee" class="key_information_detail_image" src="/helper_white.svg">
          <table>
            <tr>
              <td>Beitrag</td>
              <td>{{ data.key_information_detail_fee_amount}}</td>
            </tr>
            <tr>
              <td>IBAN:</td>
              <td>{{ data.key_information_detail_fee_iban }}</td>
            </tr>
            <tr>
              <td>Empfänger:</td>
              <td>{{ data.key_information_detail_fee_receiver }}</td>
            </tr>
            <tr>
              <td>Zweck</td>
              <td>{{ data.key_information_detail_fee_reason }}</td>
            </tr>
            <tr>
              <td>Frist</td>
              <td>{{ data.key_information_detail_fee_deadline }}</td>
            </tr>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>

.frame {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  object-fit: fill;
}

.path {
  stroke-dasharray: 1;
}

.key_information {
  color: var(--color-background);
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
  margin-bottom: auto;
  width: 3rem;
  height: 3rem;
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


th:first-child, td:first-child {
  width: 70%;
}

.info_tile {
  position: fixed;
  height: 100vh;
  min-height: 100vh;
  width: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  left: -0rem;
}

.info_tile_content {
  pointer-events: none;
  font-size: 1rem;
  position: relative;
  margin-bottom: auto;
  margin-top: auto;
  width: 25rem;
  min-width: 350px;
}

.info_tile_content h2 {
  margin: 0 0 2rem;
}

</style>
