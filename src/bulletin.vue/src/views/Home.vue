<template>
  <v-container
    class="d-flex justify-center"
    fill-height
  >
    <v-card
      outlined
      width="100%"
    >
      <v-row no-gutters>
        <v-col cols="12" class="d-flex justify-center">
          <v-card-title class="text-h5 text-xl-h1">
            Welcome to Bulletin
          </v-card-title>
        </v-col>
      </v-row>

      <v-card-actions>
        <v-row no-gutters>
          <v-spacer/>

          <v-col cols="12">
            <v-btn
              block
              text
              @click="goTo('/login')"
            >
              Login
            </v-btn>
          </v-col>

          <v-col cols="12">
            <v-btn
              block
              text
              @click="goTo('/register')"
            >
              Register
            </v-btn>
          </v-col>

          <v-spacer/>
        </v-row>
      </v-card-actions>
    </v-card>
  </v-container>
</template>

<script lang="ts">
import Vue from "vue";

export default Vue.extend({
  name: "Home",
  methods: {
    goTo(route: string) {
      this.$router.push(route);
    }
  }
});
</script>
