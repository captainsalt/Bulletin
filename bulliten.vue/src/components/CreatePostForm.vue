<template>
  <v-container>
    <v-alert v-if="errorMsg" type="error">
      {{ errorMsg }}
    </v-alert>

    <v-textarea
      v-model="content"
      solo
      auto-grow
      rows="1"
      clearable
    />

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
    content: "",
    errorMsg: ""
  }),
  methods: {
    async createPost() {
      const response = await api.createPost(this.content);

      if (!response.ok) {
        this.errorMsg = "Error posting";
        return;
      }

      this.errorMsg = "";
    }
  }
});
</script>
