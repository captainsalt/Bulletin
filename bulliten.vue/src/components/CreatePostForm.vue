<template>
  <v-container>
    <v-textarea v-model="content" solo/>

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
import * as api from "@/services/api-interface";

export default Vue.extend({
  data: () => ({
    content: ""
  }),
  methods: {
    async createPost() {
      const response = await api.createPost(this.content);

      if (response.ok)
        console.log("response :>> ", response);
      else
        console.log("error");
    }
  }
});
</script>
