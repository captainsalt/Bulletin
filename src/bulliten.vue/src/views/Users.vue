<template>
  <SideNavLayout>
    <v-container>
      <v-card
        class="mx-auto"
      >
        <div
          v-for="(u, index) in users"
          :key="u.id"
          @click="toProfile(u.username)"
        >
          <v-list-item>
            <v-list-item-content>
              <v-list-item-title>{{ u.username }}</v-list-item-title>
            </v-list-item-content>
          </v-list-item>

          <v-divider v-if="index < users.length - 1"/>
        </div>
      </v-card>
    </v-container>
  </SideNavLayout>
</template>

<script lang="ts">
import Vue from "vue";
import * as api from "@/services/api-interface";

export default Vue.extend({
  data: () => ({
    users: [] as UserAccount[]
  }),
  async beforeMount() {
    this.users = await api.getUsers();
  },
  methods: {
    async toProfile(username: string) {
      await this.$router.push(`/profile/${username}`);
    }
  }
});
</script>
