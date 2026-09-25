package com.hcmute.reservation.service;

import java.util.Map;

public interface SystemSchedulerService {
    Map<String, Object> runAllJobs(); // Dành cho @Scheduled tự động chạy
    Map<String, Object> runSpecificJob(String jobName); // Dành cho Admin gọi bằng tay qua API
}