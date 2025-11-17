import { ref, readonly } from 'vue';

const submissions = ref([]);
const isLoading = ref(false);
const error = ref(null);

export function useSubmissionsStore() {
  const setSubmissions = (data) => {
    submissions.value = data;
  };

  const addSubmission = (submission) => {
    submissions.value = [submission, ...submissions.value];
  };

  const setLoading = (loading) => {
    isLoading.value = loading;
  };

  const setError = (err) => {
    error.value = err;
  };

  const clearError = () => {
    error.value = null;
  };

  return {
    submissions: readonly(submissions),
    isLoading: readonly(isLoading),
    error: readonly(error),
    setSubmissions,
    addSubmission,
    setLoading,
    setError,
    clearError,
  };
}


