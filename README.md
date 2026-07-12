# ActionFit Identity (`com.actionfit.identity`)

프로젝트 SDK와 분리된 안정적인 설치 ID 생성, 영속 저장, 레거시 식별자 마이그레이션 흐름을 제공합니다.

## 주요 기능

- 프로젝트별 저장 방식을 연결하는 `IInstallationIdStore`
- 기존 식별자를 순서대로 확인하는 `IInstallationIdMigrationSource`
- 저장된 ID 우선 보존 → 레거시 ID 마이그레이션 → 신규 GUID 생성 순서 보장
- 저장 경로를 명시적으로 바꾸는 복구용 `ReplaceId`
- 원본 ID 없이 생성 경로를 확인하는 `InstallationIdResolutionKind`

## 기본 사용법

```csharp
using ActionFit.Identity;

var service = new InstallationIdentityService(
    new DelegateInstallationIdStore(
        () => PlayerPrefs.GetString("installation_id", ""),
        value =>
        {
            PlayerPrefs.SetString("installation_id", value);
            PlayerPrefs.Save();
        }),
    new[]
    {
        new DelegateInstallationIdMigrationSource(
            "legacy-installation-id",
            () => PlayerPrefs.GetString("legacy_installation_id", ""))
    });

string installationId = service.GetOrCreateId();
```

`InstallationIdentityService`는 유효한 저장 ID를 절대 자동 교체하지 않습니다. 저장 ID가 없을 때만 등록 순서대로 마이그레이션 소스를 확인하며, 후보도 없을 때 GUID를 생성합니다.

`ReplaceId`는 계정 복구나 충돌 해결처럼 사용자가 명시적으로 ID 교체를 선택한 흐름에서만 사용해야 합니다.

## 설치

현재 프로젝트에서는 embedded package로 사용할 수 있습니다. 수동 게시 후 다른 프로젝트의 `Packages/manifest.json`에는 다음 Git UPM 주소를 사용합니다.

```json
"com.actionfit.identity": "https://github.com/ActionFit-Editor/Identity.git#1.0.1"
```

## Unity Menu

- Package root: `Tools > Package > ActionFit Identity`
- README: `Tools > Package > ActionFit Identity > README`

## 테스트

Unity Test Framework에서 `com.actionfit.identity.Tests`를 활성화해 저장값 우선, 마이그레이션 순서, 신규 생성, 명시적 교체 및 잘못된 입력 시나리오를 검증할 수 있습니다.

## 주의사항

- 설치 ID를 인증 토큰이나 로그인 계정 ID로 사용하지 마세요.
- 로그와 분석 이벤트에 원본 설치 ID를 기록하지 마세요.
- 저장소 또는 마이그레이션 소스 읽기가 실패하면 새 ID를 임의 생성하지 말고 실패를 호출자까지 전달해 기존 사용자 매핑을 보호하세요.
