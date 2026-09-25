package com.hcmute.reservation.model.dto.reservation;

import jakarta.validation.constraints.NotEmpty;
import lombok.Data;
import java.util.List;

@Data
public class ChangeTableRequest {
    @NotEmpty
    private List<Long> tableIds;

    private String reason; // optional, để log lại lý do đổi bàn
}