<template>
  <v-container>
    <PostElement
      v-for="p in posts"
      :key="p.id"
      :post="p"
    />
  </v-container>
</template>

<script lang="ts">
import Vue from "vue";
import PostElement from "@/components/Post.vue";

export default Vue.extend({
  components: {
    PostElement
  },
  props: {
    posts: {
      type: Array,
      required: true
    }
  }
});
</script>

<style>

</style>
