<script setup>
import { useToast } from '../composables/useToast.js';

const { toasts, remove } = useToast();

const getToastClass = (type) => {
  const baseClass = 'toast';
  return `${baseClass} ${baseClass}--${type}`;
};
</script>

<template>
  <div class="toast-container">
    <TransitionGroup name="toast">
      <div
        v-for="toast in toasts"
        :key="toast.id"
        :class="getToastClass(toast.type)"
        @click="remove(toast.id)"
      >
        {{ toast.message }}
      </div>
    </TransitionGroup>
  </div>
</template>

<style scoped>
.toast-container {
  position: fixed;
  top: 1rem;
  right: 1rem;
  z-index: 9999;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.toast {
  min-width: 300px;
  padding: 1rem 1.5rem;
  border-radius: 4px;
  color: white;
  font-weight: 500;
  cursor: pointer;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  transition: transform 0.2s, opacity 0.2s;
}

.toast:hover {
  transform: translateX(-4px);
}

.toast--success {
  background: #4CAF50;
}

.toast--error {
  background: #f44336;
}

.toast--warning {
  background: #ff9800;
}

.toast--info {
  background: #2196F3;
}

.toast-enter-active,
.toast-leave-active {
  transition: all 0.3s ease;
}

.toast-enter-from {
  transform: translateX(100%);
  opacity: 0;
}

.toast-leave-to {
  transform: translateX(100%);
  opacity: 0;
}
</style>

