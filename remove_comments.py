import os
import re

def remove_comments_from_csharp(content):
    """
    Remove all types of comments from C# code while preserving string literals
    """
    result = []
    i = 0
    in_string = False
    in_char = False
    in_verbatim_string = False
    in_single_line_comment = False
    in_multi_line_comment = False
    
    while i < len(content):
        if in_single_line_comment:
            if content[i] == '\n':
                in_single_line_comment = False
                result.append('\n')
            i += 1
            continue
            
        if in_multi_line_comment:
            if i + 1 < len(content) and content[i:i+2] == '*/':
                in_multi_line_comment = False
                i += 2
                continue
            i += 1
            continue
        
        if not in_string and not in_char and not in_verbatim_string:
            if i + 2 < len(content) and content[i:i+3] == '///':
                while i < len(content) and content[i] != '\n':
                    i += 1
                continue
            
            if i + 1 < len(content) and content[i:i+2] == '//':
                in_single_line_comment = True
                i += 2
                continue
            
            if i + 1 < len(content) and content[i:i+2] == '/*':
                in_multi_line_comment = True
                i += 2
                continue
        
        if not in_char and not in_verbatim_string:
            if content[i] == '"' and (i == 0 or content[i-1] != '\\'):
                in_string = not in_string
        
        if not in_string and not in_verbatim_string:
            if content[i] == "'" and (i == 0 or content[i-1] != '\\'):
                in_char = not in_char
        
        if not in_string and not in_char:
            if i + 1 < len(content) and content[i:i+2] == '@"':
                in_verbatim_string = True
                result.append(content[i])
                i += 1
                continue
            
            if in_verbatim_string and content[i] == '"':
                if i + 1 < len(content) and content[i+1] == '"':
                    result.append('"')
                    result.append('"')
                    i += 2
                    continue
                else:
                    in_verbatim_string = False
        
        result.append(content[i])
        i += 1
    
    text = ''.join(result)
    
    lines = text.split('\n')
    cleaned_lines = []
    for line in lines:
        stripped = line.rstrip()
        cleaned_lines.append(stripped)
    
    text = '\n'.join(cleaned_lines)
    
    text = re.sub(r'\n\n\n+', '\n\n', text)
    
    return text

def process_file(filepath):
    """Process a single C# file to remove comments"""
    try:
        encodings = ['utf-8', 'utf-8-sig', 'cp1252', 'latin-1', 'iso-8859-1']
        content = None
        used_encoding = None
        
        for encoding in encodings:
            try:
                with open(filepath, 'r', encoding=encoding) as f:
                    content = f.read()
                    used_encoding = encoding
                    break
            except (UnicodeDecodeError, LookupError):
                continue
        
        if content is None:
            return False, "Could not decode file with any encoding"
        
        cleaned = remove_comments_from_csharp(content)
        
        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(cleaned)
        
        return True, None
    except Exception as e:
        return False, str(e)

def main():
    files = [
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\System\AccessibilityOptions.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\UI\LanguageScreen.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\UI\HUDManual.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\System\YieldInstructionCache.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\System\VersionDisplay.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\System\SceneTransition.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\UI\ControlsScreen.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\System\SaveSystem.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\System\OptionsData.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\UI\AccessibilityScreen.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\System\LoadingSceneManager.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\UI\Map.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\UI\MenuUI.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\UI\VideoScreen.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\UI\TutorialScreen.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\System\Camera\CameraRange.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\UI\TitleScreen.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\System\Camera\CameraController.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\UI\SoundScreen.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\UI\SelectProfileScreen.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\UI\PauseScreen.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\UI\OptionsScreen.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\System\Sound\SoundManager.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\System\GameManager.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\System\Object\ObjectPoolManager.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\System\Sound\StageMusic.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\System\Lanaguage\LanguageData.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\System\Sound\SoundSettingsManager.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\System\Lanaguage\LanguageManager.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\System\CSVReader.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\System\InputAndController\GamepadVibrationManager.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\System\InputAndController\GameInputManager.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\System\Graphic\Parallax.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\System\Enemy\DeadEnemyManager.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\System\Graphic\VideoSettingsManager.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\System\Enemy\EnemyDataManager.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\System\Enemy\EnemyData.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\System\Graphic\ScreenEffect.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\ReturnSpriteEffect.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\RandomSpriteEffects.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\Object\Portal.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\Object\HealingPiece.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\Object\TutorialTrigger.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\Actor\KnockBack.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\Actor\IActorDamage.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\Actor\ActorController.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\Actor\Actor.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\Actor\Player\Player.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\Actor\Player\PlayerDamage.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\Actor\Player\PlayerAttack.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\Actor\Enemy\BossEnemy.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\Actor\Enemy\Driller.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\Actor\Enemy\EnemyDamage.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\Actor\Enemy\EnemyBullet.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\Actor\Enemy\EnemyBodyTackle.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\Actor\Enemy\OnlyGroundPatrolEnemy.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\Actor\Enemy\EnemyAttack.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\Actor\Enemy\Enemy.cs",
        r"e:\HUMG\Năm 4\Lập trình game\Misty-Traveler-main\Assets\2.Scripts\Actor\Enemy\BossEnemyDamage.cs",
    ]
    
    success_count = 0
    errors = []
    
    for filepath in files:
        if os.path.exists(filepath):
            success, error = process_file(filepath)
            if success:
                success_count += 1
                print(f"✓ Processed: {os.path.basename(filepath)}")
            else:
                errors.append((filepath, error))
                print(f"✗ Error processing {os.path.basename(filepath)}: {error}")
        else:
            errors.append((filepath, "File not found"))
            print(f"✗ File not found: {filepath}")
    
    print(f"\n{'='*60}")
    print(f"Summary:")
    print(f"  Total files: {len(files)}")
    print(f"  Successfully processed: {success_count}")
    print(f"  Errors: {len(errors)}")
    
    if errors:
        print(f"\nErrors encountered:")
        for filepath, error in errors:
            print(f"  - {os.path.basename(filepath)}: {error}")

if __name__ == "__main__":
    main()
