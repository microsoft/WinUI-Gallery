import { Button, Checkbox, Switch, Textarea } from "@fluentui/react-components";
import * as React from "react";
import { FluentProvider, webLightTheme } from "@fluentui/react-components";
import { customStyleHooks } from "./customStyleHooks";

export const Default = () => {
  const [useWebUITheme, setUseWebUITheme] = React.useState(true);

  return (
    <FluentProvider
      customStyleHooks_unstable={useWebUITheme ? customStyleHooks : {}}
      theme={webLightTheme}
    >
      <Switch
      checked={true}
      onChange={(e, v) => setUseWebUITheme(v.checked)}
      label={useWebUITheme ? "Using WebUI Theme" : "Without WebUI Theme"}
      />
      <hr />
      <div
      style={{
        display: "flex",
        flexDirection: "column",
        alignItems: "start",
        gap: "20px",
      }}
      >
      <Button appearance="primary">Text</Button>
      <div>
        <Checkbox label="Text" />
        <Checkbox defaultChecked={true} label="Text" />
      </div>
      <Textarea />

      <Textarea defaultValue="This is Body Text. Windows is personal: it adapts seamlessly to the way I use my device." />
      </div>
    </FluentProvider>
  );
};
