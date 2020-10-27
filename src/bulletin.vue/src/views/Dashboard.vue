<template>
  <SideNavLayout>
    <v-row no-gutters>
      <v-col>
        <BulletinBoard :posts="posts"/>
      </v-col>

      <v-col cols="3">
        <div id="create-container">
          <CreatePostForm/>
        </div>
      </v-col>
    </v-row>
  </SideNavLayout>
</template>

<script lang="ts">
import Vue from "vue";
import BulletinBoard from "@/components/BulletinBoard.vue";
import CreatePostForm from "@/components/CreatePostForm.vue";
import * as api from "@/services/api-interface";
import { mapState } from "vuex";

export default Vue.extend({
  components: {
    BulletinBoard,
    CreatePostForm
  },
  data: () => ({
    posts: [] as Post[]
  }),
  computed: {
    ...mapState("auth", [
      "user"
    ])
  },
  async beforeMount() {
    this.posts = await api.getPersonalFeed();
  }
});
</script>

<style scoped>
#dash-grid {
  display: grid;
  grid-template:
    "posts" 1fr
    "form" auto / 100%;
  height: 100%;
}

#create-container {
  height: calc(100vh - 24px);
  position: sticky;
  display: flex;
  justify-content: baseline;
  top: 12px;
}

#create-container > * {
  position: absolute;
  bottom: 0;
}
</style>
