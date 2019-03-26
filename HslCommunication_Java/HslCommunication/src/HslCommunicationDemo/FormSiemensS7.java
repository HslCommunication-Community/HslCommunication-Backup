package HslCommunicationDemo;

import HslCommunication.BasicFramework.SoftBasic;
import HslCommunication.Core.Types.OperateResult;
import HslCommunication.Core.Types.OperateResultExOne;
import HslCommunication.Profinet.Siemens.SiemensPLCS;
import HslCommunication.Profinet.Siemens.SiemensS7Net;

import javax.swing.*;
import java.awt.*;
import java.awt.event.MouseAdapter;
import java.awt.event.MouseEvent;

public class FormSiemensS7 extends JDialog
{

    public FormSiemensS7(SiemensPLCS siemensPLCS){
        this.setTitle("Siemens S7 Test Tool");
        this.setSize(1020, 684);
        this.setLocationRelativeTo(null);
        this.setModal(true);
        this.siemensPLCS = siemensPLCS;

        JPanel panel = new JPanel();
        panel.setLayout(null);

        AddNormal(panel);
        AddConnectSegment(panel);
        AddContent(panel);

        this.add(panel);

        siemensS7Net = new SiemensS7Net(siemensPLCS);
    }

    private SiemensPLCS siemensPLCS = SiemensPLCS.S1200;
    private SiemensS7Net siemensS7Net = null;
    private JPanel panelContent = null;
    private String defaultAddress = "M100";


    public void AddNormal(JPanel panel){
        JLabel label1 = new JLabel("Blogs：");
        label1.setBounds(11, 9,68, 17);
        panel.add(label1);

        JLabel label5 = new JLabel("https://www.cnblogs.com/dathlin/p/9196129.html");
        label5.setBounds(80, 9,400, 17);
        panel.add(label5);

        JLabel label2 = new JLabel("Protocols");
        label2.setBounds(466, 9,68, 17);
        panel.add(label2);

        JLabel label3 = new JLabel("S7");
        label3.setForeground(Color.RED);
        label3.setBounds(540, 9,160, 17);
        panel.add(label3);

        JLabel label4 = new JLabel("By Richard Hu");
        label4.setForeground(Color.ORANGE);
        label4.setBounds(887, 9,108, 17);
        panel.add(label4);
    }

    public void AddConnectSegment(JPanel panel){
        JPanel panelConnect = new JPanel();
        panelConnect.setLayout(null);
        panelConnect.setBounds(14,32,978, 54);
        panelConnect.setBorder(BorderFactory.createTitledBorder( ""));

        JLabel label1 = new JLabel("Ip：");
        label1.setBounds(8, 17,56, 17);
        panelConnect.add(label1);

        JTextField textField1 = new JTextField();
        textField1.setBounds(62,14,106, 23);
        textField1.setText("192.168.0.10");
        panelConnect.add(textField1);

        JLabel label2 = new JLabel("Port：");
        label2.setBounds(184, 17,56, 17);
        panelConnect.add(label2);

        JTextField textField2 = new JTextField();
        textField2.setBounds(238,14,61, 23);
        textField2.setText("102");
        panelConnect.add(textField2);

        JLabel label3 = new JLabel("Rack：");
        label3.setBounds(305, 10,48, 17);
        panelConnect.add(label3);

        JTextField textField3 = new JTextField();
        textField3.setBounds(354,7,33, 23);
        textField3.setText("0");
        panelConnect.add(textField3);

        JLabel label4 = new JLabel("Slot：");
        label4.setBounds(393, 10,48, 17);
        panelConnect.add(label4);

        JTextField textField4 = new JTextField();
        textField4.setBounds(439,7,33, 23);
        textField4.setText("0");
        panelConnect.add(textField4);



        if (this.siemensPLCS == SiemensPLCS.S400)
        {
            textField3.setText("0");
            textField4.setText("3");
        }
        else if(this.siemensPLCS == SiemensPLCS.S1200)
        {
            textField3.setText("0");
            textField4.setText("0");
        }
        else if (this.siemensPLCS == SiemensPLCS.S300)
        {
            textField3.setText("0");
            textField4.setText("2");
        }
        else if (this.siemensPLCS == SiemensPLCS.S1500)
        {
            textField3.setText("0");
            textField4.setText("0");
        }



        JLabel label5 = new JLabel("Not s7-200 smart");
        label5.setBounds(337, 33,110, 17);
        label5.setForeground(Color.GRAY);
        panelConnect.add(label5);

        JButton button2 = new JButton("Disconnect");
        button2.setFocusPainted(false);
        button2.setBounds(584,11,121, 28);
        panelConnect.add(button2);

        JButton button1 = new JButton("Connect");
        button1.setFocusPainted(false);
        button1.setBounds(477,11,91, 28);
        panelConnect.add(button1);

        button2.setEnabled(false);
        button1.setEnabled(true);
        button1.addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                if (button1.isEnabled() == false)return;
                super.mouseClicked(e);
                try {
                    siemensS7Net.setIpAddress(textField1.getText());
                    siemensS7Net.setPort(Integer.parseInt(textField2.getText()));
                    if(siemensPLCS!=SiemensPLCS.S200Smart){
                        siemensS7Net.setRack((byte) Integer.parseInt(textField3.getText()));
                        siemensS7Net.setSlot((byte) Integer.parseInt(textField4.getText()));
                    }

                    OperateResult connect = siemensS7Net.ConnectServer();
                    if(connect.IsSuccess){
                        JOptionPane.showMessageDialog(
                                null,
                                "Connect Success",
                                "Result",
                                JOptionPane.PLAIN_MESSAGE);
                        DemoUtils.SetPanelEnabled(panelContent,true);
                        button2.setEnabled(true);
                        button1.setEnabled(false);
                    }
                    else {
                        JOptionPane.showMessageDialog(
                                null,
                                "Connect Failed:" + connect.ToMessageShowString(),
                                "Result",
                                JOptionPane.WARNING_MESSAGE);
                    }
                }
                catch (Exception ex){
                    JOptionPane.showMessageDialog(
                            null,
                            "Connect Failed\r\nReason:"+ex.getMessage(),
                            "Result",
                            JOptionPane.ERROR_MESSAGE);
                }
            }
        });
        button2.addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                super.mouseClicked(e);
                if (button2.isEnabled() == false) return;
                if(siemensS7Net!=null){
                    siemensS7Net.ConnectClose();
                    button1.setEnabled(true);
                    button2.setEnabled(false);
                    DemoUtils.SetPanelEnabled(panelContent,false);
                }
            }
        });


        panel.add(panelConnect);
    }

    public void AddContent(JPanel panel){
        JPanel panelContent = new JPanel();
        panelContent.setLayout(null);
        panelContent.setBounds(14,95,978, 537);
        panelContent.setBorder(BorderFactory.createTitledBorder( ""));

        AddRead(panelContent);
        AddWrite(panelContent);
        AddReadBulk(panelContent);
        AddCoreRead(panelContent);

        panel.add(panelContent);
        this.panelContent = panelContent;
        DemoUtils.SetPanelEnabled(this.panelContent,false);
    }

    public void AddRead(JPanel panel){
        JPanel panelRead = new JPanel();
        panelRead.setLayout(null);
        panelRead.setBounds(11,3,518, 234);
        panelRead.setBorder(BorderFactory.createTitledBorder( "Read Single Test"));

        JLabel label1 = new JLabel("Address：");
        label1.setBounds(9, 30,70, 17);
        panelRead.add(label1);

        JTextField textField1 = new JTextField();
        textField1.setBounds(83,27,213, 23);
        textField1.setText(defaultAddress);
        panelRead.add(textField1);

        JLabel label2 = new JLabel("Result：");
        label2.setBounds(9, 58,70, 17);
        panelRead.add(label2);

        JTextArea textArea1 = new JTextArea();
        textArea1.setLineWrap(true);
        JScrollPane jsp = new JScrollPane(textArea1);
        jsp.setBounds(83,56,213, 164);
        panelRead.add(jsp);

        JButton button1 = new JButton("r-bit");
        button1.setFocusPainted(false);
        button1.setBounds(315,19,82, 28);
        button1.addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                if (button1.isEnabled() == false) return;
                super.mouseClicked(e);
                DemoUtils.ReadResultRender(siemensS7Net.ReadBool(textField1.getText()),textField1.getText(), textArea1, jsp );
            }
        });
        panelRead.add(button1);

        JButton button2 = new JButton("r-byte");
        button2.setFocusPainted(false);
        button2.setBounds(415,19,82, 28);
        button2.addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                if (button2.isEnabled() == false) return;
                super.mouseClicked(e);
                DemoUtils.ReadResultRender(siemensS7Net.ReadByte(textField1.getText()),textField1.getText(), textArea1, jsp );
            }
        });
        panelRead.add(button2);

        JButton button3 = new JButton("r-short");
        button3.setFocusPainted(false);
        button3.setBounds(315,56,82, 28);
        button3.addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                if (button3.isEnabled() == false) return;
                super.mouseClicked(e);
                DemoUtils.ReadResultRender(siemensS7Net.ReadInt16(textField1.getText()),textField1.getText(), textArea1, jsp );
            }
        });
        panelRead.add(button3);

        JButton button4 = new JButton("r-ushort");
        button4.setFocusPainted(false);
        button4.setVisible(false);
        button4.setBounds(415,56,82, 28);
        panelRead.add(button4);

        JButton button5 = new JButton("r-int");
        button5.setFocusPainted(false);
        button5.setBounds(315,90,82, 28);
        button5.addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                if (button5.isEnabled() == false) return;
                super.mouseClicked(e);
                DemoUtils.ReadResultRender(siemensS7Net.ReadInt32(textField1.getText()),textField1.getText(), textArea1, jsp );
            }
        });
        panelRead.add(button5);

        JButton button6 = new JButton("r-uint");
        button6.setFocusPainted(false);
        button6.setVisible(false);
        button6.setBounds(415,90,82, 28);
        panelRead.add(button6);

        JButton button7 = new JButton("r-long");
        button7.setFocusPainted(false);
        button7.setBounds(315,124,82, 28);
        button7.addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                if (button7.isEnabled() == false) return;
                super.mouseClicked(e);
                DemoUtils.ReadResultRender(siemensS7Net.ReadInt64(textField1.getText()),textField1.getText(), textArea1, jsp );
            }
        });
        panelRead.add(button7);

        JButton button8 = new JButton("r-ulong");
        button8.setFocusPainted(false);
        button8.setVisible(false);
        button8.setBounds(415,124,82, 28);
        panelRead.add(button8);

        JButton button9 = new JButton("r-float");
        button9.setFocusPainted(false);
        button9.setBounds(315,158,82, 28);
        button9.addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                if (button9.isEnabled() == false) return;
                super.mouseClicked(e);
                DemoUtils.ReadResultRender(siemensS7Net.ReadFloat(textField1.getText()),textField1.getText(), textArea1, jsp );
            }
        });
        panelRead.add(button9);

        JButton button10 = new JButton("r-double");
        button10.setFocusPainted(false);
        button10.setBounds(415,158,82, 28);
        button10.addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                if (button10.isEnabled() == false) return;
                super.mouseClicked(e);
                DemoUtils.ReadResultRender(siemensS7Net.ReadDouble(textField1.getText()),textField1.getText(), textArea1, jsp );
            }
        });
        panelRead.add(button10);

        JButton button11 = new JButton("r-string");
        button11.setFocusPainted(false);
        button11.setBounds(415,192,82, 28);
        button11.addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                if (button11.isEnabled() == false) return;
                super.mouseClicked(e);
                DemoUtils.ReadResultRender(siemensS7Net.ReadString(textField1.getText()),textField1.getText(), textArea1, jsp );
            }
        });
        panelRead.add(button11);

        panel.add(panelRead);
    }

    public void AddWrite(JPanel panel){
        JPanel panelWrite = new JPanel();
        panelWrite.setLayout(null);
        panelWrite.setBounds(546,3,419, 234);
        panelWrite.setBorder(BorderFactory.createTitledBorder( "Write Single Test"));

        JLabel label1 = new JLabel("Address：");
        label1.setBounds(9, 30,70, 17);
        panelWrite.add(label1);

        JTextField textField1 = new JTextField();
        textField1.setBounds(83,27,132, 23);
        textField1.setText(defaultAddress);
        panelWrite.add(textField1);

        JLabel label2 = new JLabel("Value：");
        label2.setBounds(9, 58,70, 17);
        panelWrite.add(label2);

        JTextField textField2 = new JTextField();
        textField2.setBounds(83,56,132, 23);
        panelWrite.add(textField2);

        JLabel label100 = new JLabel("Note: The value of \r\nthe string needs to be converted");
        label100.setBounds(61, 82,147, 58);
        panelWrite.add(label100);

        JButton button1 = new JButton("w-bit");
        button1.setFocusPainted(false);
        button1.setBounds(226,24,82, 28);
        button1.addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                if (button1.isEnabled() == false) return;
                super.mouseClicked(e);
                try {
                    DemoUtils.WriteResultRender(siemensS7Net.Write(textField1.getText(), Boolean.parseBoolean(textField2.getText())), textField1.getText());
                }
                catch (Exception ex){
                    JOptionPane.showMessageDialog(
                            null,
                            "Write Failed:" + ex.getMessage(),
                            "Result",
                            JOptionPane.ERROR_MESSAGE);
                }
            }
        });
        panelWrite.add(button1);

        JButton button2 = new JButton("w-byte");
        button2.setFocusPainted(false);
        button2.setBounds(326,24,82, 28);
        button2.addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                if (button2.isEnabled() == false) return;
                super.mouseClicked(e);
                try {
                    DemoUtils.WriteResultRender(siemensS7Net.Write(textField1.getText(), Byte.parseByte(textField2.getText())), textField1.getText());
                }
                catch (Exception ex){
                    JOptionPane.showMessageDialog(
                            null,
                            "Write Failed:" + ex.getMessage(),
                            "Result",
                            JOptionPane.ERROR_MESSAGE);
                }
            }
        });
        panelWrite.add(button2);

        JButton button3 = new JButton("w-short");
        button3.setFocusPainted(false);
        button3.setBounds(226,61,82, 28);
        button3.addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                if (button3.isEnabled() == false) return;
                super.mouseClicked(e);
                try {
                    DemoUtils.WriteResultRender(siemensS7Net.Write(textField1.getText(), Short.parseShort(textField2.getText())), textField1.getText());
                }
                catch (Exception ex){
                    JOptionPane.showMessageDialog(
                            null,
                            "Write Failed:" + ex.getMessage(),
                            "Result",
                            JOptionPane.ERROR_MESSAGE);
                }
            }
        });
        panelWrite.add(button3);

        JButton button4 = new JButton("w-ushort");
        button4.setFocusPainted(false);
        button4.setVisible(false);
        button4.setBounds(326,61,82, 28);
        panelWrite.add(button4);

        JButton button5 = new JButton("w-int");
        button5.setFocusPainted(false);
        button5.setBounds(226,95,82, 28);
        button5.addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                if (button5.isEnabled() == false) return;
                super.mouseClicked(e);
                try {
                    DemoUtils.WriteResultRender(siemensS7Net.Write(textField1.getText(), Integer.parseInt(textField2.getText())), textField1.getText());
                }
                catch (Exception ex){
                    JOptionPane.showMessageDialog(
                            null,
                            "Write Failed:" + ex.getMessage(),
                            "Result",
                            JOptionPane.ERROR_MESSAGE);
                }
            }
        });
        panelWrite.add(button5);

        JButton button6 = new JButton("w-uint");
        button6.setFocusPainted(false);
        button6.setVisible(false);
        button6.setBounds(326,95,82, 28);
        panelWrite.add(button6);

        JButton button7 = new JButton("w-long");
        button7.setFocusPainted(false);
        button7.setBounds(226,129,82, 28);
        button7.addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                if (button7.isEnabled() == false) return;
                super.mouseClicked(e);
                try {
                    DemoUtils.WriteResultRender(siemensS7Net.Write(textField1.getText(), Long.parseLong(textField2.getText())), textField1.getText());
                }
                catch (Exception ex){
                    JOptionPane.showMessageDialog(
                            null,
                            "Write Failed:" + ex.getMessage(),
                            "Result",
                            JOptionPane.ERROR_MESSAGE);
                }
            }
        });
        panelWrite.add(button7);

        JButton button8 = new JButton("w-ulong");
        button8.setFocusPainted(false);
        button8.setVisible(false);
        button8.setBounds(326,129,82, 28);
        panelWrite.add(button8);

        JButton button9 = new JButton("w-float");
        button9.setFocusPainted(false);
        button9.setBounds(226,163,82, 28);
        button9.addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                if (button9.isEnabled() == false) return;
                super.mouseClicked(e);
                try {
                    DemoUtils.WriteResultRender(siemensS7Net.Write(textField1.getText(), Float.parseFloat(textField2.getText())), textField1.getText());
                }
                catch (Exception ex){
                    JOptionPane.showMessageDialog(
                            null,
                            "Write Failed:" + ex.getMessage(),
                            "Result",
                            JOptionPane.ERROR_MESSAGE);
                }
            }
        });
        panelWrite.add(button9);

        JButton button10 = new JButton("w-double");
        button10.setMargin(new Insets(0,0,0,0));
        button10.setFocusPainted(false);
        button10.setBounds(326,163,82, 28);
        button10.addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                if (button10.isEnabled() == false) return;
                super.mouseClicked(e);
                try {
                    DemoUtils.WriteResultRender(siemensS7Net.Write(textField1.getText(), Double.parseDouble(textField2.getText())), textField1.getText());
                }
                catch (Exception ex){
                    JOptionPane.showMessageDialog(
                            null,
                            "Write Failed:" + ex.getMessage(),
                            "Result",
                            JOptionPane.ERROR_MESSAGE);
                }
            }
        });
        panelWrite.add(button10);

        JButton button11 = new JButton("w-string");
        button11.setFocusPainted(false);
        button11.setBounds(326,197,82, 28);
        button11.addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                if (button11.isEnabled() == false) return;
                super.mouseClicked(e);
                try {
                    DemoUtils.WriteResultRender(siemensS7Net.Write(textField1.getText(), textField2.getText()), textField1.getText());
                }
                catch (Exception ex){
                    JOptionPane.showMessageDialog(
                            null,
                            "Write Failed:" + ex.getMessage(),
                            "Result",
                            JOptionPane.ERROR_MESSAGE);
                }
            }
        });
        panelWrite.add(button11);

        panel.add(panelWrite);
    }

    public void AddReadBulk(JPanel panel){
        JPanel panelRead = new JPanel();
        panelRead.setLayout(null);
        panelRead.setBounds(11,243,518, 154);
        panelRead.setBorder(BorderFactory.createTitledBorder( "Read byte by Length"));

        JLabel label1 = new JLabel("Address：");
        label1.setBounds(9, 30,70, 17);
        panelRead.add(label1);

        JTextField textField1 = new JTextField();
        textField1.setBounds(83,27,82, 23);
        textField1.setText(defaultAddress);
        panelRead.add(textField1);

        JLabel label2 = new JLabel("Length：");
        label2.setBounds(185, 30,60, 17);
        panelRead.add(label2);

        JTextField textField2 = new JTextField();
        textField2.setBounds(234,27,102, 23);
        textField2.setText("10");
        panelRead.add(textField2);


        JLabel label3 = new JLabel("Result：");
        label3.setBounds(9, 58,70, 17);
        panelRead.add(label3);

        JTextArea textArea1 = new JTextArea();
        textArea1.setLineWrap(true);
        JScrollPane jsp = new JScrollPane(textArea1);
        jsp.setBounds(83,56,425, 78);
        panelRead.add(jsp);

        JButton button1 = new JButton("Order");
        button1.setFocusPainted(false);
        button1.setBounds(342,24,82, 28);
        button1.addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                if (button1.isEnabled() == false) return;
                super.mouseClicked(e);
                OperateResultExOne<String> read = siemensS7Net.ReadOrderNumber();
                if(read.IsSuccess){
                    textArea1.setText(read.Content);
                }
                else {
                    JOptionPane.showMessageDialog(
                            null,
                            "Read Failed:" + read.ToMessageShowString(),
                            "Result",
                            JOptionPane.ERROR_MESSAGE);
                }
            }
        });
        panelRead.add(button1);

        JButton button2 = new JButton("Read");
        button2.setFocusPainted(false);
        button2.setBounds(426,24,82, 28);
        button2.addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                if (button2.isEnabled() == false) return;
                super.mouseClicked(e);
                OperateResultExOne<byte[]> read = siemensS7Net.Read(textField1.getText(),Short.parseShort(textField2.getText()));
                if(read.IsSuccess){
                    textArea1.setText(SoftBasic.ByteToHexString(read.Content));
                }
                else {
                    JOptionPane.showMessageDialog(
                            null,
                            "Read Failed:" + read.ToMessageShowString(),
                            "Result",
                            JOptionPane.ERROR_MESSAGE);
                }
            }
        });
        panelRead.add(button2);

        panel.add(panelRead);
    }

    public void AddCoreRead(JPanel panel){
        JPanel panelRead = new JPanel();
        panelRead.setLayout(null);
        panelRead.setBounds(11,403,518, 118);
        panelRead.setBorder(BorderFactory.createTitledBorder( "报文读取测试-Full message read"));

        JLabel label1 = new JLabel("Message：");
        label1.setBounds(9, 30,70, 17);
        panelRead.add(label1);

        JTextField textField1 = new JTextField();
        textField1.setBounds(83,27,337, 23);
        textField1.setText(SoftBasic.ByteToHexString(SiemensS7Net.BuildReadCommand(defaultAddress,(short) 1).Content, ' '));
        panelRead.add(textField1);

        JLabel label3 = new JLabel("Result：");
        label3.setBounds(9, 58,70, 17);
        panelRead.add(label3);

        JTextArea textArea1 = new JTextArea();
        textArea1.setLineWrap(true);
        JScrollPane jsp = new JScrollPane(textArea1);
        jsp.setBounds(83,56,425, 52);
        panelRead.add(jsp);

        JButton button2 = new JButton("Read");
        button2.setFocusPainted(false);
        button2.setBounds(426,24,82, 28);
        button2.addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                if (button2.isEnabled() == false) return;
                super.mouseClicked(e);
                OperateResultExOne<byte[]> read = siemensS7Net.ReadFromCoreServer(SoftBasic.HexStringToBytes(textField1.getText()));
                if(read.IsSuccess){
                    textArea1.setText(SoftBasic.ByteToHexString(read.Content));
                }
                else {
                    JOptionPane.showMessageDialog(
                            null,
                            "Read Failed:" + read.ToMessageShowString(),
                            "Result",
                            JOptionPane.ERROR_MESSAGE);
                }
            }
        });
        panelRead.add(button2);

        panel.add(panelRead);
    }
}
