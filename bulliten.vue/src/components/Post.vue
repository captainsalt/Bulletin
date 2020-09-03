<template>
  <div>
    {{ post.content }}
  </div>
</template>

<script lang="ts">
import Vue, { PropType } from "vue";

export default Vue.extend({
  props: {
    post: {
      type: Object as PropType<Post>,
      required: true
    }
  }
});
</script>
