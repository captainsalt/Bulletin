<template>
  <v-container fill-height>
    <v-row no-gutters>
      <v-col sm="8">
        <BullitenBoard/>
      </v-col>

      <v-col>
        <div id="grid">
          <CreatePostForm class="create"/>
        </div>
      </v-col>
    </v-row>
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
.create {
  grid-area: create;
}

#grid {
  height: 98vh;
  display: grid;
  position: sticky;
  top: 0;
  grid-template:
    "." 5fr
    "create" 1fr / 100%;
}
</style>
