const vm = new Vue({
    el: '#app',
    async created() {
        const { status: clusterStatus, data: clusterData } = await this.$http.get('/api/services/quartz/Scheduler/cluster');
        if (clusterStatus === 200) {
            this.schedulerClusters = clusterData;
        }
        this.getJobData();
        this.getTriggerData();
    },
    data: {
        activeName: 'first',
        searchForm: {
            instanceName: 'QuartzScheduler'
        },
        triggerDialogVisible: false,
        editJobDialogVisible: false,
        rescheduleDialogVisible: false,
        schedulerClusters: [],
        jobData: [],
        triggerData: [],
        scheduledTriggerData: [],
        jobDetailData: {},
        scheduledJobData: {}
    },
    methods: {
        handleClick(tab, event) {
            console.log(tab, event);
        },
        async getJobData() {
            const { status: status, data: data } = await this.$http.get('/api/services/quartz/Jobs');
            if (status === 200) {
                this.jobData = data;
            }
        },
        async getTriggerData() {
            const { status: status, data: data } = await this.$http.get('/api/services/quartz/Triggers');
            if (status === 200) {
                this.triggerData = data
            }
        },
        showTriggers(row) {
            this.scheduledTriggerData = [];
            this.triggerData.forEach(x => {
                if (x.jobName === row.jobName) {
                    this.scheduledTriggerData.push(x)
                }
            })
            this.triggerDialogVisible = true;
        },
        reschedule(row) {
            var idx = this.triggerData.findIndex(x => x.jobName === row.jobName);

            var trigger = {}
            if (idx >= 0) {
                trigger = this.triggerData[idx];
            }

            this.scheduledJobData = {
                jobName: row.jobName,
                jobGroup: row.jobGroup,
                jobDescription: row.description,
                jobData: row.jobData,
                triggerName: trigger.triggerName,
                triggerGroup: trigger.triggerGroup,
                triggerDescription: trigger.description,
                cronExpression: trigger.cronExpression
            };
            this.rescheduleDialogVisible = true;
        },
        editJob(row) {
            this.jobDetailData = row
            this.editJobDialogVisible = true
        },
        deleteJob() {

        },
        async doRescheduleJob() {

            const { status: status, data: data } = await this.$http.put(
                `/api/services/quartz/Triggers/${this.scheduledJobData.triggerName}/${this.scheduledJobData.triggerGroup}`,
                {
                    jobName: this.scheduledJobData.jobName,
                    jobGroup: this.scheduledJobData.jobGroup,
                    description: this.scheduledJobData.triggerDescription,
                    cronExpression: this.scheduledJobData.cronExpression
                });

            if (status === 200) {
                this.getJobData();
                this.getTriggerData();
                this.rescheduleDialogVisible = false;
            }
        },
        async saveJob() {
            const { status: status, data: data } = await this.$http.put(
                `/api/services/quartz/Jobs/${this.jobDetailData.jobName}/${this.jobDetailData.jobGroup}`, {
                description: this.jobDetailData.description,
                jobData: this.jobDetailData.jobData
            });
            if (status === 200) {
                this.getJobData();
                this.editJobDialogVisible = false;
            }
        }
    }
})