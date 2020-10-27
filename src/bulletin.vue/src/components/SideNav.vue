<template>
  <v-card outlined>
    <v-navigation-drawer
      floating
      permanent
    >
      <v-list>
        <v-list-item>
          <v-list-item-content>
            <v-list-item-title class="title">
              Hello, {{ user.username }}
            </v-list-item-title>
          </v-list-item-content>
        </v-list-item>

        <v-divider/>

        <v-list-item
          v-for="item in items"
          :key="item.title"
          link
          @click="navigate(item.path)"
        >
          <v-list-item-icon>
            <v-icon>{{ item.icon }}</v-icon>
          </v-list-item-icon>

          <v-list-item-content>
            <v-list-item-title>
              {{ item.title }}
            </v-list-item-title>
          </v-list-item-content>
        </v-list-item>
      </v-list>

      <template v-slot:append>
        <div class="pa-2">
          <v-btn
            color="error"
            outlined
            block
            @click="logout"
          >
            Logout
          </v-btn>
        </div>
      </template>
    </v-navigation-drawer>
  </v-card>
</template>

<script lang="ts">
import Vue from "vue";
import { mapState, mapActions } from "vuex";

export default Vue.extend({
  computed: {
    ...mapState("auth", [
      "user"
    ]),
    items() {
      return [
        { title: "Dashboard", icon: "mdi-view-dashboard", path: "/dashboard" },
        { title: "Profile", icon: "mdi-account", path: `/profile/${this.user.username}` },
        { title: "Users", icon: "mdi-account-group", path: "/users" }
      ];
    }
  },
  methods: {
    ...mapActions("auth", [
      "logout"
    ]),
    async navigate(path: string) {
      if (path === this.$route.path)
        return;

      await this.$router.push(path);
    }
  }
});
</script>
