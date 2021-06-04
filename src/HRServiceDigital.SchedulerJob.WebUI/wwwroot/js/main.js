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
        jobDialogVisible: false,
        isEditJob: false,
        isRescheduleJob: false,
        rescheduleDialogVisible: false,
        scheduleJobDialogVisible: false,
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
        addJob() {
            this.isEditJob = false;
            this.jobDetailData = {
                jobGroup: 'DEFAULT'
            };
            this.jobDialogVisible = true;
        },
        async saveAddJob() {
            const { status: status } = await this.$http.post('/api/services/quartz/Jobs', {
                jobGroup: this.jobDetailData.jobGroup,
                description: this.jobDetailData.description,
                jobData: this.jobDetailData.jobData
            });

            if (status === 200) {
                this.$message({
                    type: 'success',
                    message: 'New Job Created Successfully!',
                    offset: 60
                });
                this.getJobData();
                this.jobDialogVisible = false;
            } else {
                this.$message({ type: 'error', message: 'Job Creation Failed!', offset: 60 });
            }
        },
        editJob() {
            this.isEditJob = true;
            var selectedJobData = this.$refs.jobDataTable.selection
            if (selectedJobData.length !== 1) {
                this.$message.error('Please select one of the job data.');
                return;
            }

            this.jobDetailData = selectedJobData[0]
            this.jobDialogVisible = true
        },
        async saveJob() {
            const { status: status } = await this.$http.put(
                `/api/services/quartz/Jobs/${this.jobDetailData.jobName}/${this.jobDetailData.jobGroup}`, {
                description: this.jobDetailData.description,
                jobData: this.jobDetailData.jobData
            });
            if (status === 200) {
                this.$message({ type: 'success', message: 'Job save Successfully!', offset: 60 });
                this.getJobData();
                this.jobDialogVisible = false;
            } else {
                this.$message({ type: 'error', message: 'Job save Failed!', offset: 60 });
            }
        },
        addTrigger() {
            this.isRescheduleJob = false;
            var selectedJobData = this.$refs.jobDataTable.selection
            if (selectedJobData.length !== 1) {
                this.$message.error('Please select one of the job data.');
                return;
            }
            var row = selectedJobData[0];

            if (this.triggerData.findIndex(x => x.jobName === row.jobName) >= 0) {
                this.$message({ type: 'error', message: 'The selected job has been assigned with trigger, no need to create more!', offset: 60});
                return;
            }
            this.scheduledJobData = {
                jobName: row.jobName,
                jobGroup: row.jobGroup,
                jobDescription: row.description,
                jobData: row.jobData,
                triggerGroup: 'DEFAULT'
            };
            this.rescheduleDialogVisible = true;
        },
        async addTriggerConfirm() {
            const { status: status } = await this.$http.post('/api/services/quartz/Triggers', {
                triggerGroup: this.scheduledJobData.triggerGroup,
                jobName: this.scheduledJobData.jobName,
                jobGroup: this.scheduledJobData.jobGroup,
                description: this.scheduledJobData.triggerDescription,
                cronExpression: this.scheduledJobData.cronExpression
            });

            if (status === 200) {
                this.$message({ type: 'success', message: 'Add Trigger Successfully!', offset: 60 });
                this.getJobData();
                this.getTriggerData();
                this.rescheduleDialogVisible = false;
            } else {
                this.$message({ type: 'error', message: 'Add Trigger Failed!', offset: 60 });
            }
        },
        deleteJob(name, group) {
            this.$confirm('Are you going to delete the job as well as associated triggers?', 'Notice', {
                confirmButtonText: 'Yes',
                cancelButtonText: 'Cancel',
                type: 'warning'
            }).then(async () => {
                const { status: status } = await this.$http.delete(`/api/services/quartz/Jobs/${name}/${group}`);
                if (status === 200) {
                    this.$message({
                        type: 'success',
                        message: 'Delete Job Successfully!',
                        offset: 60
                    });
                    this.getJobData();
                    this.getTriggerData();
                    this.jobDialogVisible = false;
                } else {
                    this.$message({ type: 'error', message: 'Delete Job Failed!', offset: 60 });
                }
            }).catch(() => { })
        },
        removeTrigger(row) {
            this.$confirm('Are you going to remove the trigger?', 'Notice', {
                confirmButtonText: 'Yes',
                cancelButtonText: 'Cancel',
                type: 'warning'
            }).then(async () => {
                const { status: status } = await this.$http.delete(`api/services/quartz/Triggers/${row.triggerName}/${row.triggerGroup}`);
                if (status === 200) {
                    this.$message({
                        type: 'success',
                        message: 'Remove Successfully!',
                        offset: 60
                    });
                    var triggerData = this.scheduledTriggerData
                    this.scheduledTriggerData = triggerData.filter(x => x.triggerName !== row.triggerName);
                    this.getTriggerData();
                } else {
                    this.$message({ type: 'error', message: 'Remove Trigger Failed!', offset: 60 });
                }
            }).catch(() => { })
        },
        scheduleJob() {
            this.isRescheduleJob = false;
            this.scheduledJobData = {
                jobGroup: 'DEFAULT',
                triggerGroup: 'DEFAULT'
            };
            this.scheduleJobDialogVisible = true;
        },
        async saveScheduleJob() {
            const { status: status } = await this.$http.post('/api/services/quartz/Jobs/schedulejob', {
                jobGroup: this.scheduledJobData.jobGroup,
                jobDescription: this.scheduledJobData.jobDescription,
                jobData: this.scheduledJobData.jobData,
                triggerGroup: this.scheduledJobData.triggerGroup,
                triggerDescription: this.scheduledJobData.triggerDescription,
                cronExpression: this.scheduledJobData.cronExpression
            });
            if (status === 200) {
                this.$message({ type: 'success', message: 'Schedule Job Successfully!', offset: 60 });
                this.getJobData();
                this.getTriggerData();
                this.scheduleJobDialogVisible = false;
            } else {
                this.$message({ type: 'error', message: 'Schedule Job Failed!', offset: 60 });
            }
        },
        reschedule() {
            this.isRescheduleJob = true;
            var selectedJobData = this.$refs.jobDataTable.selection
            if (selectedJobData.length !== 1) {
                this.$message({type: 'error', message: 'Please select one of the job data.', offset: 60});
                return;
            }
            var row = selectedJobData[0]
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
        async doRescheduleJob() {

            const { status: status } = await this.$http.put(
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
            } else {
                this.$message({ type: 'error', message: 'Reschedule Job Failed!', offset: 60 });
            }
        }
    }
})