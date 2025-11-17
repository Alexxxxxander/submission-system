<script setup>
import { ref, onMounted } from 'vue';
import { api } from '../services/api.js';
import { ApiError } from '../services/httpClient.js';
import { useToast } from '../composables/useToast.js';
import { translateFieldName, translateValue } from '../utils/translations.js';

const submissions = ref([]);
const searchQuery = ref('');
const isLoading = ref(false);
const { error: showError } = useToast();

const loadSubmissions = async () => {
  isLoading.value = true;
  try {
    submissions.value = await api.getSubmissions(searchQuery.value || undefined);
  } catch (err) {
    if (err instanceof ApiError) {
      showError(err.message);
    } else {
      showError('Failed to load submissions list');
    }
    console.error('Failed to load submissions:', err);
  } finally {
    isLoading.value = false;
  }
};

const handleSearch = () => {
  loadSubmissions();
};

const formatDate = (dateString) => {
  return new Date(dateString).toLocaleString('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  });
};

const parseData = (dataString) => {
  try {
    if (!dataString || dataString.length > 100000) {
      return {};
    }
    return JSON.parse(dataString);
  } catch {
    return {};
  }
};

const getTranslatedFieldName = (field) => translateFieldName(field);
const getTranslatedValue = (field, value) => translateValue(field, value);

onMounted(() => {
  loadSubmissions();
});

defineExpose({
  loadSubmissions,
});
</script>

<template>
  <div class="submissions-list">
    <h2>Submissions List</h2>

    <div class="search-bar">
      <input
        v-model="searchQuery"
        type="text"
        placeholder="Search submissions..."
        @keyup.enter="handleSearch"
      />
      <button @click="handleSearch" class="search-btn">Search</button>
    </div>

    <div v-if="isLoading" class="loading">Loading...</div>

    <div v-else-if="submissions.length === 0" class="empty-state">
      No submissions yet
    </div>

    <div v-else class="submissions-grid">
      <div v-for="submission in submissions" :key="submission.id" class="submission-card">
        <div class="submission-header">
          <span class="form-type">{{ submission.formType }}</span>
          <span class="submitted-date">{{ formatDate(submission.submittedAt) }}</span>
        </div>
        <div class="submission-data">
          <div v-for="(value, key) in parseData(submission.data)" :key="key" class="data-row">
            <span class="data-label">{{ getTranslatedFieldName(String(key)) }}</span>
            <span class="data-value">{{ getTranslatedValue(String(key), value) }}</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.submissions-list {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem;
}

h2 {
  margin: 0 0 2rem 0;
  color: #1a1a1a;
  font-size: 1.5rem;
  font-weight: 600;
}

.search-bar {
  display: flex;
  gap: 0.75rem;
  margin-bottom: 2rem;
  max-width: 600px;
}

.search-bar input {
  flex: 1;
  padding: 0.75rem 1rem;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  font-size: 0.95rem;
  transition: all 0.2s;
}

.search-bar input:focus {
  outline: none;
  border-color: #4CAF50;
  box-shadow: 0 0 0 3px rgba(76, 175, 80, 0.1);
}

.search-btn {
  padding: 0.75rem 1.5rem;
  background: #4CAF50;
  color: white;
  border: none;
  border-radius: 8px;
  font-size: 0.95rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.search-btn:hover {
  background: #45a049;
  transform: translateY(-1px);
}

.search-btn:active {
  transform: translateY(0);
}

.loading,
.empty-state {
  text-align: center;
  padding: 4rem 2rem;
  color: #666;
  font-size: 1rem;
}

.submissions-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: 1.25rem;
}

.submission-card {
  background: #fff;
  border: 1px solid #f0f0f0;
  border-radius: 12px;
  padding: 1.5rem;
  transition: all 0.2s;
}

.submission-card:hover {
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.08);
  border-color: #e0e0e0;
}

.submission-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
  padding-bottom: 1rem;
  border-bottom: 1px solid #f5f5f5;
}

.form-type {
  font-weight: 600;
  color: #4CAF50;
  text-transform: uppercase;
  font-size: 0.75rem;
  letter-spacing: 0.5px;
  background: rgba(76, 175, 80, 0.1);
  padding: 0.25rem 0.75rem;
  border-radius: 4px;
}

.submitted-date {
  color: #999;
  font-size: 0.8rem;
}

.submission-data {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.data-row {
  display: grid;
  grid-template-columns: 140px 1fr;
  gap: 1rem;
  align-items: start;
}

.data-label {
  color: #666;
  font-size: 0.85rem;
  font-weight: 500;
  text-align: left;
}

.data-value {
  color: #1a1a1a;
  font-size: 0.95rem;
  word-break: break-word;
  text-align: left;
}

@media (max-width: 768px) {
  .submissions-list {
    padding: 1rem;
  }
  
  .submissions-grid {
    grid-template-columns: 1fr;
  }
  
  .data-row {
    grid-template-columns: 110px 1fr;
    gap: 0.75rem;
  }
}
</style>

