from django.contrib import admin
from django.urls import include, re_path, path

urlpatterns = [
    path(r"admin/", admin.site.urls),
    path(r'o/', include('oauth2_provider.urls', namespace='oauth2_provider')),
    re_path('^api/(?P<version>(v1|v2))/', include('auth_a.urls')),
]
