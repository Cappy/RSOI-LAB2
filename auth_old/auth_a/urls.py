from django.urls import path

from auth_a.views import ApiEndpoint

urlpatterns = [
    path(r'hello/', ApiEndpoint.as_view()),  # an example resource endpoint
]
