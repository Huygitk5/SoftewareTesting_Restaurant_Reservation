package com.hcmute.reservation.service.impl;

import com.hcmute.reservation.scheduler.ScheduledJob;
import com.hcmute.reservation.service.SystemSchedulerService;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Service;

import java.util.HashMap;
import java.util.Map;

@Slf4j
@Service
@RequiredArgsConstructor
public class SystemSchedulerServiceImpl implements SystemSchedulerService {

    // "tableReleaseJob" -> đối tượng TableReleaseJob
    private final Map<String, ScheduledJob> jobRegistry;

    @Override
    @Scheduled(fixedDelay = 60_000)
    public Map<String, Object> runAllJobs() {
        log.info("[Scheduler] Bắt đầu chạy tất cả các jobs...");
        Map<String, Object> results = new HashMap<>();

        for (Map.Entry<String, ScheduledJob> entry : jobRegistry.entrySet()) {
            try {
                results.put(entry.getKey(), entry.getValue().execute());
            } catch (Exception e) {
                log.error("[Scheduler] Lỗi job {}: {}", entry.getKey(), e.getMessage());
            }
        }
        return results;
    }

    @Override
    public Map<String, Object> runSpecificJob(String jobName) {
        // Tìm Job dựa trên tên Admin truyền vào
        ScheduledJob job = jobRegistry.get(jobName);

        if (job == null) {
            throw new IllegalArgumentException("Không tìm thấy Job nào có tên: " + jobName);
        }

        log.info("[Manual Trigger] Đang chạy bằng tay job: {}", jobName);
        return job.execute();
    }
}