package com.hcmute.reservation.config;

import org.springframework.context.annotation.Configuration;
import org.springframework.web.servlet.config.annotation.ResourceHandlerRegistry;
import org.springframework.web.servlet.config.annotation.WebMvcConfigurer;
import java.io.File;
import java.nio.file.Paths;

@Configuration
public class WebConfig implements WebMvcConfigurer {

    @Override
    public void addResourceHandlers(ResourceHandlerRegistry registry) {
        // Ánh xạ URL /web-customer/** vào thư mục vật lý web-customer của project
        String projectRoot = System.getProperty("user.dir");
        // Nếu user.dir trỏ vào reservation-api, ta cần lên 2 cấp để ra root, nếu backend-api thì lên 1 cấp
        File rootDir = new File(projectRoot);
        if (rootDir.getName().equals("reservation-api")) {
             projectRoot = rootDir.getParentFile().getParentFile().getAbsolutePath();
        } else if (rootDir.getName().equals("backend-api")) {
             projectRoot = rootDir.getParentFile().getAbsolutePath();
        }
        
        // Sử dụng Paths.get(...).toUri().toString() để tạo định dạng file:/// an toàn trên cả Windows/Linux
        String frontendPath = Paths.get(projectRoot, "web-customer").toUri().toString();
        // Xóa trailing slash nếu có để đồng nhất cấu trúc, sau đó cộng thêm "/"
        if (!frontendPath.endsWith("/")) {
            frontendPath += "/";
        }
        
        registry.addResourceHandler("/web-customer/**")
                .addResourceLocations(frontendPath);
                
        // Cho phép truy cập cả các file assets, js trong đó
        registry.addResourceHandler("/assets/**")
                .addResourceLocations(frontendPath + "assets/");
        registry.addResourceHandler("/js/**")
                .addResourceLocations(frontendPath + "js/");
    }
}
