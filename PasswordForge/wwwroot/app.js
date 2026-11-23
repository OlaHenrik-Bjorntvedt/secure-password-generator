const { createApp } = Vue;

createApp({
  data() {
    return {
      options: {
        length: 16,
        useLowercase: true,
        useUppercase: true,
        useNumbers: true,
        useSpecialCharacters: true
      },
      password: '',
      strength: '',
      error: '',
      loading: false,
      copied: false
    };
  },
  methods: {
    async generatePassword() {
      this.error = '';
      this.copied = false;
      this.loading = true;

      try {
        const response = await fetch('/api/passwords', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(this.options)
        });
        if (!response.headers.get('content-type')?.includes('application/json')) {
          throw new Error(
            'The password API is unavailable. From the repository folder, run ' +
            '"dotnet run --project PasswordForge -- --web" and open the URL printed ' +
            'in the terminal instead of Live Server.'
          );
        }

        const result = await response.json();

        if (!response.ok) {
          throw new Error(result.error || 'Password generation failed.');
        }

        this.password = result.password;
        this.strength = result.strength;
      } catch (error) {
        this.error = error instanceof TypeError
          ? 'Could not reach PasswordForge. Check that the server is running and try again.'
          : error.message;
      } finally {
        this.loading = false;
      }
    },
    async copyPassword() {
      try {
        await navigator.clipboard.writeText(this.password);
        this.copied = true;
        window.setTimeout(() => { this.copied = false; }, 1800);
      } catch {
        this.error = 'Could not copy the password. Please copy it manually.';
      }
    }
  }
}).mount('#app');
