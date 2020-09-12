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
import Vue, { PropType } from "vue";
import PostElement from "@/components/Post.vue";

export default Vue.extend({
  components: {
    PostElement
  },
  props: {
    posts: {
      type: Array as PropType<Post[]>,
      required: true
    }
  }
});
</script>

<style>

</style>
