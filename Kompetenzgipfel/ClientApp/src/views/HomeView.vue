<template>
  <div>
    <button @click="postData">Send POST Request</button>
    <button @click="getData">Send GET Request</button>
  </div>
  <div>
    <p id="outputter">{{ apiResult }}</p>
  </div>
</template>

<script lang="ts" setup>
import {ref} from 'vue';
import axios from 'axios';

const apiResult = ref("not fetched yet");

const postData = async () => {
  try {
    const response = await axios.post('/api/topic/AddNew', {
      "Title": "generic title",
      "Description": "generic description"
    });
    apiResult.value = response.data;
  } catch (error) {
    console.log(error)
  }
};

const getData = async () => {
  try {
    const response = await axios.get('/api/topic/all');
    apiResult.value = response.data;
  } catch (error) {
    // Handle error if needed
  }
};
</script>
