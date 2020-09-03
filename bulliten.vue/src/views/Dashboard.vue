<template>
  <v-container fill-height>
    <BullitenBoard class="bulliten"/>
    <CreatePostForm class="create"/>
  </v-container>
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
.bulliten {
  height: 100%;
}

.create {
  position: absolute;
  bottom: 0;
}
</style>
