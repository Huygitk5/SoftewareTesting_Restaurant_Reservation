package com.hcmute.reservation.model.dto.table;

import com.hcmute.reservation.model.enums.TableStatus;
import jakarta.validation.constraints.Min;
import jakarta.validation.constraints.NotNull;
import lombok.Data;

@Data
public class TableRequest {
    @NotNull @Min(1)
    private Integer capacity;

    private Boolean isActive = true;
    // Bổ sung thêm status để quản lý có thể cập nhật trạng thái thủ công (trong service)
    private TableStatus status;
    // Bổ sung version để hỗ trợ Optimistic Locking khi update (trong service)
    private Long version;
}
