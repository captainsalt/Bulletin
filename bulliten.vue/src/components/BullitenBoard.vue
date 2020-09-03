<template>
  <div>
    <PostElement
      v-for="p in posts"
      :key="p.id"
      :post="p"
    />
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import PostElement from "@/components/Post.vue";
import { getPosts } from "@/services/api-interface";

export default Vue.extend({
  components: {
    PostElement
  },
  data: () => ({
    posts: [] as Array<Post>
  }),
  async beforeMount() {
    this.posts = (await getPosts());
  }
});
</script>

<style>

</style>
