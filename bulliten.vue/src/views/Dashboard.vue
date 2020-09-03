<template>
  <div>
    <BullitenBoard/>
    <CreatePostForm/>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import CreatePostForm from "@/components/CreatePostForm.vue";
import BullitenBoard from "@/components/BullitenBoard.vue";

export default Vue.extend({
  components: {
    CreatePostForm,
    BullitenBoard
  }
});
</script>

<style>

</style>
