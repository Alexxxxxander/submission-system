<script setup>
import { ref } from 'vue';
import OrderForm from './components/OrderForm.vue';
import SubmissionsList from './components/SubmissionsList.vue';
import ToastContainer from './components/ToastContainer.vue';

const submissionsListRef = ref(null);

const handleFormSubmitted = () => {
  submissionsListRef.value?.loadSubmissions();
};
</script>

<template>
  <div class="app">
    <ToastContainer />
    
    <header class="app-header">
      <h1>Form Submission System</h1>
    </header>

    <main class="app-main">
      <section class="form-section">
        <OrderForm @submitted="handleFormSubmitted" />
      </section>

      <section class="list-section">
        <SubmissionsList ref="submissionsListRef" />
      </section>
    </main>
  </div>
</template>

<style>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
  background: #fafafa;
  color: #1a1a1a;
  line-height: 1.6;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}

#app {
  min-height: 100vh;
}
</style>

<style scoped>
.app {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.app-header {
  background: #fff;
  border-bottom: 1px solid #f0f0f0;
  color: #1a1a1a;
  padding: 1.5rem 2rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
}

.app-header h1 {
  font-size: 1.5rem;
  font-weight: 600;
  max-width: 1200px;
  margin: 0 auto;
}

.app-main {
  flex: 1;
  padding: 2rem 1rem;
}

.form-section {
  margin-bottom: 3rem;
}

@media (max-width: 768px) {
  .app-header {
    padding: 1rem;
  }
  
  .app-header h1 {
    font-size: 1.25rem;
  }
  
  .app-main {
    padding: 1rem 0.5rem;
  }
}
</style>
