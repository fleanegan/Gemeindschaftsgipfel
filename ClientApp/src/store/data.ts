import {defineStore} from 'pinia';

export const useDataStore = defineStore({
    id: 'data',
    state: () => ({
	isLoading: false,
	content: 'initialized',
    }),
    actions: {
	startLoading() {
	    this.isLoading = true;
	},
	stopLoading() {
	    this.isLoading = false;
	},
	conti(){
	    this.content = 'content written'
	    console.log('content has been written')
	}
    },
});
