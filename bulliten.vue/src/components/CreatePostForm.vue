<template>
  <v-container>
    <v-textarea solo/>

    <v-btn
      color="primary"
      block
      @click="createPost"
    >
      Post
    </v-btn>
  </v-container>
</template>

<script lang="ts">
import Vue from "vue";
export default Vue.extend({
  methods: {
    createPost() {

    }
  }
});
</script>
