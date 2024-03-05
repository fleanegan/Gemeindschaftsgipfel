<template>
  <h1>Template</h1>
  <div class="instruction_container">
    <div class="instruction_card">
      <div class="instruction_card_content">
        <p class="instruction_card_content_header_title" style="padding-left: 1.5rem">Inhalt</p>
        <p class="instruction_card_content_header_title" style="padding-left: 1rem; padding-bottom: 0.5rem">
          ausdenken</p>
        <p>Reite dein Steckenpferd und erzaehle uns, was dich begeistert! Ob
          Trick
          17, dein Promotionsthema oder Haekeltipps, wir sind gespannt.</p>
      </div>
      <div class="instruction_card_enumerator">1.</div>
    </div>
    <div class="instruction_card">
      <div class="instruction_card_content">
        <p class="instruction_card_content_header_title" style="padding-left: 1.5rem">Entscheidung</p>
        <p class="instruction_card_content_header_title" style="padding-left: 1rem; padding-bottom: 0.5rem">
          treffen</p>
        <p>Geht es dir wie uns, du kannst dich kaum entscheiden, welches deiner vielen Herzensthemen du praesentieren
          sollst? Trag alle Themen ein, lass die Gemeindschaft waehlen und hilf selbst mit deiner Stimme!</p>
      </div>
      <div class="instruction_card_enumerator">2.</div>
    </div>
    <div class="instruction_card">
      <div class="instruction_card_content">
        <p class="instruction_card_content_header_title" style="padding-left: 1.5rem">Gemeinsam</p>
        <p class="instruction_card_content_header_title" style="padding-left: 1rem; padding-bottom: 0.5rem">
          staunen</p>
        <p>Das Ziel ist es, zusammen unsere Vielfalt zu geniessen. Lass Leistungsdruck und Lampenfieber zuhause, denn
          es
          erwartet dich ein wohlwollendes Publikum :)</p>
      </div>
      <div class="instruction_card_enumerator">3.</div>
    </div>
  </div>
</template>


<script lang="ts">
import {defineComponent} from 'vue';

export default defineComponent({
  data() {
    return {};
  },
  methods: {},
  computed: {}
});
</script>

<style scoped>
.instruction_container {
  display: flex;
  box-sizing: border-box;
  flex-direction: row;
  justify-items: center;
  margin-right: 1rem;
}

.instruction_card {
  position: relative;
}

.instruction_card_content_header_title {
  font-size: 0.75rem;
  font-stretch: extra-expanded;
  font-weight: bolder;
}

.instruction_card_content {
  margin-left: 1rem;
  margin-top: 1rem;
  padding-right: 0.5rem;
  padding-left: 1rem;
  border-style: none;
  border-radius: 0.2rem;
  font-size: 0.5rem;
  height: 100%;
  background-color: var(--color-main-accent);
  color: var(--color-background);
}

.instruction_card_enumerator {
  position: absolute;
  left: 0;
  top: 0;
  height: 3rem;
  width: 3rem;
  background-color: var(--color-main-accent);
  border: 0.2rem;
  border-radius: 3rem;
  border-style: solid;
  border-color: var(--color-background);
  color: var(--color-background);
  font-size: 1.75rem;
  font-weight: bold;
  text-align: center;
}
</style>