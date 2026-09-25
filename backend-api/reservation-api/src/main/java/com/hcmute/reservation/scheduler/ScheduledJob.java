package com.hcmute.reservation.scheduler;

import java.util.Map;

public interface ScheduledJob {
    Map<String, Object> execute();
}
